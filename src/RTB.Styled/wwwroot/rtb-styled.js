// rtb-styled.js — CSSOM-focused, for *scoped* CSS (C# already scoped)
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

    // Main entry for the C#-scoped pipeline
    injectScoped(css, cls) {
        const tag = this.ensureTag();
        const sheet = tag.sheet;
        if (!css || !cls) return;

        // If no CSSOM (very rare / locked), fall back to text replace
        if (sheet) {
            this.clearRule(cls);
            tag.append(css);
            return;
        }

        // 1) Remove previous .cls rules (inside groups too)
        this.clearRule(cls);

        // 2) Proactively remove any @keyframes (by name) that appear in the incoming css
        const rules = this.splitTopLevelRules(css);
        const kfNames = [];
        for (const r of rules) {
            const name = this.keyframesName(r);
            if (name) kfNames.push(name);
        }
        for (const name of kfNames) {
            this.removeKeyframes(sheet, name);
        }

        // 3) Insert each top-level rule via CSSOM, fallback to text append if needed
        for (const ruleText of rules) {
            this.insertRuleSafe(sheet, ruleText);
        }
    },

    // Remove prior rules for this class everywhere
    clearRule(cls) {
        const tag = document.getElementById("rtb-styled");
        if (!tag || !tag.sheet) return;
        const selector = '.' + cls;

        const removeIn = (container) => {
            if (!container || !container.cssRules) return;
            // walk backwards to avoid index shifts
            for (let i = container.cssRules.length - 1; i >= 0; i--) {
                const r = container.cssRules[i];
                if (r.type === CSSRule.STYLE_RULE && r.selectorText === selector) {
                    container.deleteRule(i);
                } else if ('cssRules' in r) {
                    removeIn(r);
                    // If the group becomes empty, keep it—browsers handle empty @media fine.
                }
            }
        };
        removeIn(tag.sheet);
    },

    clearAll() {
        const tag = document.getElementById("rtb-styled");
        if (tag) tag.innerHTML = '';
    },

    // ---------- helpers ----------

    insertRuleSafe(sheet, ruleText) {
        const txt = (ruleText || '').trim();
        if (!txt) return;

        try {
            sheet.insertRule(txt, sheet.cssRules.length);
        } catch (e) {
            // Some engines get picky (e.g. vendor-specific descriptors). Fallback to text append.
            console && console.debug && console.debug('insertRule fallback:', e, txt);
            sheet.ownerNode.append(txt);
        }
    },

    // Split a stylesheet string into *top-level* rules:
    // ".a{...}", "@media {...}", "@keyframes k{...}", etc. (no trailing semicolons)
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
                    // skip trailing ws/semicolons between rules
                    i++;
                    while (i < css.length && (css[i] === ';' || /\s/.test(css[i]))) i++;
                    start = i;
                    continue;
                }
            }
            i++;
        }
        const tail = css.slice(start).trim();
        if (tail) rules.push(tail);
        return rules.filter(Boolean);
    },

    // Extract selector of a normal style rule (null for at-rules)
    selectorOfRule(ruleText) {
        const m = /^([^{@][^{]+)\{/.exec(ruleText);
        return m ? m[1].trim() : null;
    },

    // Find @keyframes name (handles -webkit- too)
    keyframesName(ruleText) {
        let m = /^@keyframes\s+([a-zA-Z0-9_-]+)/.exec(ruleText);
        if (m) return m[1];
        m = /^@-webkit-keyframes\s+([a-zA-Z0-9_-]+)/.exec(ruleText);
        return m ? m[1] : null;
    },

    // Remove existing keyframes of a given name (both standard and -webkit-)
    removeKeyframes(sheet, name) {
        const KEY = (CSSRule && CSSRule.KEYFRAMES_RULE) || 7;
        const WEBKIT_KEY = (CSSRule && CSSRule.WEBKIT_KEYFRAMES_RULE) || 7;

        for (let i = sheet.cssRules.length - 1; i >= 0; --i) {
            const r = sheet.cssRules[i];
            if ((r.type === KEY || r.type === WEBKIT_KEY) && r.name === name) {
                sheet.deleteRule(i);
            }
        }
    }
};