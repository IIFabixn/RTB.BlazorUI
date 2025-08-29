window.rtbStyled = {
    inject(css) {
        let tag = document.getElementById("rtb-styled");
        if (!tag) {
            tag = document.createElement("style");
            tag.id = "rtb-styled";
            document.head.appendChild(tag);
        }

        // ‼️  delete each selector that already exists
        const ruleRegex = /([^{]+)\{/g;          // finds ".s-123ABC{" etc.
        let m;
        while ((m = ruleRegex.exec(css)) !== null) {
            const selector = m[1].trim();          // ".s-123ABC"
            this.removeRule(tag, selector);
        }

        tag.append(css); // append once
    },

    injectInto(css, cls) {
        let tag = document.getElementById("rtb-styled");
        if (!tag) {
            tag = document.createElement("style");
            tag.id = "rtb-styled";
            document.head.appendChild(tag);
        }

        const selector = `.${cls}`;
        const sheet = tag.sheet;

        // Fallback if we can’t get the CSSOM
        if (!sheet) { tag.append(`${selector}${css}`); return; }

        this.removeRule(tag, selector);

        // Create fresh CSS rule
        try {
            var rule = `${selector}${css}`;
            sheet.insertRule(rule, sheet.cssRules.length);
        } catch (e) {
            console.log(e);
            // Fallback: append as text if insertRule fails
            tag.append(`${selector}${css}`);
        }
    },

    removeRule(styleTag, selector) {
        const sheet = styleTag.sheet;
        if (!sheet) return;
        for (let i = sheet.cssRules.length - 1; i >= 0; --i) {
            const r = sheet.cssRules[i];
            if ('selectorText' in r && r.selectorText === selector) {
                sheet.deleteRule(i);
                // don’t break if you might have duplicates
                break;
            }
        }
    },

    clearRule(cls) {
        const tag = document.getElementById("rtb-styled");
        if (tag) this.removeRule(tag, '.' + cls);
    },

    clearAll() {
        const tag = document.getElementById("rtb-styled");
        if (tag) tag.innerHTML = ''; // Clear all styles
    },

    sanitizeCss(css) {
        // remove empty @media blocks: @media ... {}
        css = css.replace(/@media[^{]+\{\s*\}/g, '');
        // collapse any accidental double braces like "{{}}"
        css = css.replace(/\{\s*\{+/g, '{').replace(/\}\s*\}+/g, '}');
        return css.trim();
    }
};