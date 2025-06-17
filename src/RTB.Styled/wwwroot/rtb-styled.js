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

        if (!sheet) {
            // Fallback: append as text if sheet is not accessible
            tag.append(`${selector}{${css}}`);
            return;
        }

        this.removeRule(tag, selector);

        // Create fresh CSS rule
        try {
            var rule = `${selector}{${css}}`;
            sheet.insertRule(rule, sheet.cssRules.length);
        } catch (e) {
            // Fallback: append as text if insertRule fails
            tag.append(`${selector}{${css}}`);
        }
    },

    removeRule(styleTag, selector) {
        const sheet = styleTag.sheet;
        if (!sheet) return;
        for (let i = sheet.cssRules.length - 1; i >= 0; --i) {
            if (sheet.cssRules[i].selectorText === selector) {
                sheet.deleteRule(i);
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
    }
};