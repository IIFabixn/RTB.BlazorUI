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

        // If no CSSOM (very rare), just append text
        if (!sheet) { tag.append(css); return; }

        const rules = this.splitTopLevelRules(css);
        for (const ruleText of rules) {
            this.upsertRule(sheet, ruleText);
        }
    },

    injectInto(css, cls) {
        // css should be a block like "{prop:val; &:hover{...}}"
        css = this.sanitizeCss(css);
        const tag = this.ensureTag();
        const sheet = tag.sheet;
        const selector = `.${cls}`;

        // Extract any @keyframes accidentally included and insert them separately
        // (so we can still safely call insertRule for the class rule)
        const { baseRule, atRules } = this.extractAtRules(css);

        if (sheet) {
            // Fallback: append text (cannot de-dupe)
            tag.append(`${selector}${baseRule}${atRules.join('')}`);
            return;
        }

        // Remove prior .class rule (if any)
        this.removeRuleBySelector(sheet, selector);

        // Upsert the base rule
        try {
            sheet.insertRule(`${selector}${baseRule}`, sheet.cssRules.length);
        } catch (e) {
            console.log(e);
            tag.append(`${selector}${baseRule}`); // last-resort fallback
        }

        // Insert any @keyframes that came along
        for (const ar of atRules) {
            this.upsertRule(sheet, ar);
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
        css = css.replace(/@media[^{]+\{\s*\}/g, '');
        return css.trim();
    },

    // --- Internals ----------------------------------------------------------

    // Split a stylesheet text into top-level rules: ".a{...}", "@keyframes k{...}", "@media{...}", etc.
    splitTopLevelRules(css) {
        const rules = [];
        let i = 0, start = 0, depth = 0;
        while (i < css.length) {
            const ch = css[i];
            if (ch === '{') depth++;
            else if (ch === '}') {
                depth--;
                if (depth === 0) {
                    rules.push(css.slice(start, i + 1).trim());
                    // skip any trailing whitespace/semicolons
                    i++;
                    while (i < css.length && /\s|;/.test(css[i])) i++;
                    start = i;
                    continue;
                }
            }
            i++;
        }
        // trailing junk (shouldn't happen)
        const tail = css.slice(start).trim();
        if (tail) rules.push(tail);
        return rules.filter(Boolean);
    },

    // If css is like "{...}@keyframes x{...}@keyframes y{...}", pull out at-rules
    extractAtRules(css) {
        const rules = this.splitTopLevelRules(css.replace(/^\s*/, ''));
        const atRules = [];
        let baseRule = css;

        // Heuristic: the base block is the first block starting with '{'
        // @-rules are complete rules starting with '@'
        const chunks = this.splitTopLevelRules(css);
        if (chunks.length) {
            const first = chunks[0];
            if (first.startsWith('{')) {
                baseRule = first;
                atRules.push(...chunks.slice(1).filter(r => r.startsWith('@')));
            } else {
                // no base rule, everything are at-rules
                baseRule = '';
                atRules.push(...chunks);
            }
        }

        return { baseRule, atRules };
    },

    upsertRule(sheet, ruleText) {
        const trimmed = ruleText.trim();
        if (!trimmed) return;

        if (trimmed.startsWith('@keyframes')) {
            const name = this.keyframesName(trimmed);
            if (name) this.removeKeyframes(sheet, name);
            // insert as is
            try {
                sheet.insertRule(trimmed, sheet.cssRules.length);
            } catch (e) {
                // Some engines only accept vendor-prefixed constants; fallback to text append
                console.log(e);
                sheet.ownerNode.append(trimmed);
            }
            return;
        }

        // Other at-rules that are group rules (@media, @supports, …) or normal rules
        // For normal rules, try to dedupe by selectorText if possible
        const sel = this.selectorOfRule(trimmed);
        if (sel) this.removeRuleBySelector(sheet, sel);

        try {
            sheet.insertRule(trimmed, sheet.cssRules.length);
        } catch (e) {
            console.log(e);
            sheet.ownerNode.append(trimmed); // fallback
        }
    },

    selectorOfRule(ruleText) {
        // crude but safe enough: get text before first '{'
        const m = /^([^{@][^{]+)\{/.exec(ruleText);
        if (!m) return null;
        return m[1].trim();
    },

    keyframesName(ruleText) {
        const m = /^@keyframes\s+([a-zA-Z0-9_-]+)/.exec(ruleText);
        if (m) return m[1];
        // try webkit-prefixed
        const w = /^@-webkit-keyframes\s+([a-zA-Z0-9_-]+)/.exec(ruleText);
        return w ? w[1] : null;
    },

    removeRuleBySelector(sheet, selector) {
        // Walk backwards so deletions don't shift indices
        for (let i = sheet.cssRules.length - 1; i >= 0; --i) {
            const r = sheet.cssRules[i];
            if (r.type === CSSRule.STYLE_RULE && r.selectorText === selector) {
                sheet.deleteRule(i);
                // break; // keep going in case of duplicates
            } else if ('cssRules' in r) {
                // Recurse into @media / @supports etc.
                this.removeRuleBySelector(r, selector);
            }
        }
    },

    removeKeyframes(sheet, name) {
        const KEY = CSSRule.KEYFRAMES_RULE ?? 7;             // fallback numeric
        const WEBKIT_KEY = CSSRule.WEBKIT_KEYFRAMES_RULE ?? 7;

        for (let i = sheet.cssRules.length - 1; i >= 0; --i) {
            const r = sheet.cssRules[i];
            if ((r.type === KEY || r.type === WEBKIT_KEY) && r.name === name) {
                sheet.deleteRule(i);
            }
        }
    }
};
