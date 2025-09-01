window.rtbStyled = {
    ensureTag() {
        let tag = document.getElementById("rtb-styled");
        if (!tag) {
            tag = document.createElement("style");
            tag.id = "rtb-styled";
            document.head.appendChild(tag);
        }
        return tag;
    },

    // --- Public -------------------------------------------------------------

    inject(css) {
        css = this.sanitizeCss(css);
        const tag = this.ensureTag();
        const sheet = tag.sheet;

        // If no CSSOM, just append raw text
        if (!sheet) { tag.append(css); return; }

        // Parse and upsert every top-level node (style rule, group rule, keyframes, etc.)
        const nodes = this.parseStylesheet(css);
        for (const node of nodes) {
            this.upsertNode(sheet, node);
        }
    },

    /**
     * Scoped injection: given a class name, scope all “bare blocks” and nested style blocks to it.
     * Accepts:
     *   css like "{...; &:hover{...}} @media (min-width: 992px){{max-width:992px;}}"
     */
    injectInto(css, cls) {
        css = this.sanitizeCss(css);
        const tag = this.ensureTag();
        const sheet = tag.sheet;
        const selector = `.${cls}`;

        // 1) Parse the snippet to a node array (can be a mix of base block + at-rules)
        const nodes = this.parseStylesheet(css);

        // 2) Scope nodes: expand "&", wrap bare blocks with selector, recurse into groups
        const scoped = nodes.map(n => this.scopeNode(n, selector));

        // 3) De-dupe and upsert
        if (!sheet) {
            // Text fallback if CSSOM unavailable
            tag.append(scoped.map(n => this.stringifyNode(n)).join(''));
            return;
        }

        // For a clean slate for this specific selector at top-level, remove any plain rules with same selector
        this.removeRuleBySelector(sheet, selector);

        for (const node of scoped) {
            this.upsertNode(sheet, node);
        }
    },

    clearRule(cls) {
        const tag = document.getElementById("rtb-styled");
        if (!tag || !tag.sheet) return;
        this.removeRuleBySelector(tag.sheet, '.' + cls);
    },

    clearAll() {
        const tag = document.getElementById("rtb-styled");
        if (tag) tag.innerHTML = '';
    },

    sanitizeCss(css) {
        // Remove empty @media blocks (safely)
        css = css.replace(/@media[^{]+\{\s*\}/g, '');
        return css.trim();
    },

    // --- Parser -------------------------------------------------------------

    /**
     * Minimal CSS block parser sufficient for rules, at-rules, and nested groups.
     * Returns an array of AST nodes:
     *   { type: 'style', selector: '.a, .b', block: 'prop:val; ...' }
     *   { type: 'group', name: '@media', prelude: 'screen and (min-width: 992px)', children: [nodes...] }
     *   { type: 'keyframes', name: 'spin', vendor: '', cssText: '...full @keyframes text...' }
     *   { type: 'bare', block: 'prop:val; &:hover{...}' }   // a “{...}” without selector (to be scoped)
     */
    parseStylesheet(css) {
        const nodes = [];
        let i = 0, n = css.length;

        const skipWS = () => { while (i < n && /\s/.test(css[i])) i++; };
        const peek = () => css[i];
        const readUntil = (pred) => { let s = ''; while (i < n && !pred(css[i])) s += css[i++]; return s; };

        const readBalancedBlock = () => {
            // assumes current char is '{'
            let depth = 0, start = i, end = i;
            while (i < n) {
                const ch = css[i++];
                if (ch === '{') depth++;
                else if (ch === '}') {
                    depth--;
                    if (depth === 0) { end = i; break; }
                }
            }
            return css.slice(start, end); // includes outer '{...}'
        };

        while (i < n) {
            skipWS();
            if (i >= n) break;

            if (peek() === '@') {
                // at-rule
                const at = readUntil(ch => ch === '{' || ch === ';' || ch === '\n');
                if (i < n && css[i] === '{') {
                    // group rule or keyframes-like with block
                    const block = readBalancedBlock();
                    const header = at.trim(); // e.g., "@media screen and (min-width: 992px)"
                    const full = `${header}${block}`;
                    // detect keyframes
                    if (/^@(-webkit-)?keyframes\b/i.test(header)) {
                        // keep whole text as-is for exact fidelity
                        nodes.push({ type: 'keyframes', cssText: full, name: this.keyframesName(header) || '', vendor: header.startsWith('@-webkit-') ? '-webkit-' : '' });
                    } else {
                        const { name, prelude } = this.splitAtHeader(header);
                        const inner = block.slice(1, -1); // remove outer braces
                        const children = this.parseStylesheet(inner);
                        nodes.push({ type: 'group', name, prelude, children });
                    }
                } else {
                    // at-rule without block (e.g., @charset, @import)
                    const text = at + (css[i] === ';' ? css[i++] : '');
                    nodes.push({ type: 'raw', cssText: text });
                }
            } else if (peek() === '{') {
                // bare block -> to be scoped later
                const block = readBalancedBlock();
                nodes.push({ type: 'bare', block: block });
            } else {
                // selector + block
                const selectorText = readUntil(ch => ch === '{').trim();
                if (i >= n || css[i] !== '{') {
                    // stray text; treat as raw
                    const rest = selectorText + readUntil(_ => false);
                    nodes.push({ type: 'raw', cssText: rest });
                    break;
                }
                const block = readBalancedBlock();
                nodes.push({ type: 'style', selector: selectorText, block });
            }
        }

        return nodes.filter(Boolean);
    },

    splitAtHeader(header) {
        // header like: "@media screen and (min-width: 992px)"
        const m = /^(@[a-z-]+)\s*(.*)$/i.exec(header.trim());
        return m ? { name: m[1], prelude: (m[2] || '').trim() } : { name: header.trim(), prelude: '' };
    },

    keyframesName(header) {
        const m = /^@(?:-webkit-)?keyframes\s+([a-zA-Z0-9_-]+)/.exec(header);
        return m ? m[1] : null;
    },

    // --- Scoping ------------------------------------------------------------

    /**
     * Scope a parsed node to a selector:
     *  - expand '&' in selectors and blocks
     *  - wrap bare blocks as `${selector}{...}`
     *  - recurse into group rules
     *  - leave keyframes/raw untouched
     */
    scopeNode(node, selector) {
        switch (node.type) {
            case 'bare': {
                const block = this.expandAmp(node.block, selector);
                return { type: 'style', selector, block };
            }
            case 'style': {
                const sel = this.expandAmpInSelector(node.selector, selector);
                const block = this.expandAmp(node.block, selector);
                return { type: 'style', selector: sel, block };
            }
            case 'group': {
                const children = node.children.map(ch => this.scopeNode(ch, selector));
                return { ...node, children };
            }
            case 'keyframes':
            case 'raw':
            default:
                return node;
        }
    },

    expandAmpInSelector(sel, selector) {
        // Replace all standalone "&" or "&..." cases properly
        return sel.replace(/&/g, selector);
    },
    expandAmp(block, selector) {
        return block.replace(/&/g, selector);
    },

    // --- Stringifier --------------------------------------------------------

    stringifyNode(node) {
        switch (node.type) {
            case 'style':
                return `${node.selector}${node.block}`;
            case 'group':
                return `${node.name}${node.prelude ? ' ' + node.prelude : ''}{${node.children.map(n => this.stringifyNode(n)).join('')}}`;
            case 'keyframes':
                return node.cssText;
            case 'raw':
                return node.cssText;
            default:
                return '';
        }
    },

    // --- CSSOM Upsert & De-dupe --------------------------------------------

    upsertNode(sheet, node) {
        if (!node) return;

        if (node.type === 'keyframes') {
            const name = this.keyframesName(node.cssText) || node.name || '';
            if (name) this.removeKeyframes(sheet, name);
            this.safeInsertRule(sheet, node.cssText);
            return;
        }

        if (node.type === 'style') {
            // De-dupe by selector at top-level
            const selector = node.selector.trim();
            if (selector) this.removeRuleBySelector(sheet, selector);
            this.safeInsertRule(sheet, this.stringifyNode(node));
            return;
        }

        if (node.type === 'group') {
            // Insert empty group (or find existing) then reconcile children
            const groupSig = `${node.name} ${node.prelude}`.trim();
            // We’ll attempt to upsert by:
            //  1) searching for an identical group (same type & prelude) at top level,
            //  2) if found, mutate its inner rules (delete duplicates by selector), then insert children;
            //  3) if not found, insert entire group as text (fallback), *or* insert each child via CSSOM if engine exposes the GroupRule API.
            const existingGroup = this.findGroupRule(sheet, node.name, node.prelude);

            if (!existingGroup) {
                // Try to insert as a whole text; if that fails, fall back to per-child insertion after creating the group
                const text = this.stringifyNode({ ...node, children: [] });
                const ok = this.safeInsertRule(sheet, text);
                const target = this.findGroupRule(sheet, node.name, node.prelude);

                if (target && 'insertRule' in target) {
                    // De-dupe and insert children into the real group
                    for (const child of node.children) {
                        this.upsertIntoGroup(target, child);
                    }
                    return;
                }

                // Fallback: cannot access created group via CSSOM – append as raw text
                if (!ok) {
                    sheet.ownerNode.append(this.stringifyNode(node));
                } else {
                    // If we inserted the empty group successfully but can't mutate it, append children as text after it
                    sheet.ownerNode.append(node.children.map(n => this.stringifyNode(n)).join(''));
                }
                return;
            }

            // Group exists: upsert children into it
            for (const child of node.children) {
                this.upsertIntoGroup(existingGroup, child);
            }
            return;
        }

        if (node.type === 'raw') {
            this.safeInsertRule(sheet, node.cssText);
            return;
        }
    },

    upsertIntoGroup(groupRule, node) {
        if (!node) return;

        // If nested group inside group: recurse if API exposes cssRules
        if (node.type === 'group') {
            // Try to find an existing nested group
            const existing = this.findNestedGroupRule(groupRule, node.name, node.prelude);
            if (existing) {
                for (const child of node.children) this.upsertIntoGroup(existing, child);
                return;
            }
            // Insert empty nested group and then populate
            const header = `${node.name}${node.prelude ? ' ' + node.prelude : ''}{}`;
            try {
                groupRule.insertRule(header, groupRule.cssRules.length);
                const created = this.findNestedGroupRule(groupRule, node.name, node.prelude);
                if (created) {
                    for (const child of node.children) this.upsertIntoGroup(created, child);
                    return;
                }
            } catch (e) {
                console.log(e);
                // Fallback: append full text to owner <style>
                groupRule.parentStyleSheet.ownerNode.append(this.stringifyNode(node));
            }
            return;
        }

        if (node.type === 'style') {
            // De-dupe by selector **inside** this group
            this.removeRuleBySelector(groupRule, node.selector.trim());
            try {
                groupRule.insertRule(this.stringifyNode(node), groupRule.cssRules.length);
            } catch (e) {
                console.log(e);
                groupRule.parentStyleSheet.ownerNode.append(this.stringifyNode(node));
            }
            return;
        }

        if (node.type === 'keyframes' || node.type === 'raw') {
            // Keyframes are not children of media/supports per spec, but if present, upsert at sheet level
            const sheet = groupRule.parentStyleSheet;
            if (node.type === 'keyframes') {
                const name = this.keyframesName(node.cssText) || '';
                if (name) this.removeKeyframes(sheet, name);
            }
            this.safeInsertRule(sheet, this.stringifyNode(node));
        }
    },

    safeInsertRule(sheetOrGroup, text) {
        try {
            sheetOrGroup.insertRule(text, sheetOrGroup.cssRules.length);
            return true;
        } catch (e) {
            console.log(e);
            try {
                // Some engines only accept certain constructs; fallback to appending raw text
                const sheet = 'parentStyleSheet' in sheetOrGroup ? sheetOrGroup.parentStyleSheet : sheetOrGroup;
                sheet.ownerNode.append(text);
            } catch { }
            return false;
        }
    },

    // --- De-dupe helpers ----------------------------------------------------

    selectorOfRule(ruleText) {
        const m = /^([^{@][^{]+)\{/.exec(ruleText);
        return m ? m[1].trim() : null;
    },

    removeRuleBySelector(container, selector) {
        // container: CSSStyleSheet or CSSGroupingRule
        if (!('cssRules' in container)) return;
        for (let i = container.cssRules.length - 1; i >= 0; --i) {
            const r = container.cssRules[i];
            if (r.type === CSSRule.STYLE_RULE && r.selectorText === selector) {
                container.deleteRule(i);
            } else if ('cssRules' in r) {
                this.removeRuleBySelector(r, selector);
            }
        }
    },

    removeKeyframes(sheet, name) {
        const KEY = CSSRule.KEYFRAMES_RULE ?? 7;
        const WEBKIT_KEY = CSSRule.WEBKIT_KEYFRAMES_RULE ?? 7;
        for (let i = sheet.cssRules.length - 1; i >= 0; --i) {
            const r = sheet.cssRules[i];
            if ((r.type === KEY || r.type === WEBKIT_KEY) && r.name === name) {
                sheet.deleteRule(i);
            }
        }
    },

    findGroupRule(sheet, atName, prelude) {
        if (!sheet || !sheet.cssRules) return null;
        for (let i = 0; i < sheet.cssRules.length; i++) {
            const r = sheet.cssRules[i];
            const match = this.groupMatches(r, atName, prelude);
            if (match) return r;
        }
        return null;
    },

    findNestedGroupRule(group, atName, prelude) {
        if (!group || !group.cssRules) return null;
        for (let i = 0; i < group.cssRules.length; i++) {
            const r = group.cssRules[i];
            const match = this.groupMatches(r, atName, prelude);
            if (match) return r;
        }
        return null;
    },

    groupMatches(rule, atName, prelude) {
        // Handle @media, @supports, @container, @layer (when represented)
        if (!('cssRules' in rule)) return false;

        const name = atName.toLowerCase();
        if (name === '@media' && rule.conditionText !== undefined) {
            return rule.constructor?.name?.includes('MediaRule') && (rule.media?.mediaText || rule.conditionText) === prelude;
        }
        if (name === '@supports' && rule.conditionText !== undefined) {
            return rule.constructor?.name?.includes('SupportsRule') && rule.conditionText === prelude;
        }
        if (name === '@container' && rule.conditionText !== undefined) {
            return rule.constructor?.name?.includes('ContainerRule') && rule.conditionText === prelude;
        }
        if (name === '@layer') {
            // Many engines flatten @layer; best-effort string compare
            // Note: CSSLayerBlockRule isn’t widely exposed; fallback to text compare if available
            const text = rule.cssText || '';
            return text.startsWith(`@layer ${prelude}`);
        }
        return false;
    },
};
