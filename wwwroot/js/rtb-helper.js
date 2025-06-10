
window.initDragDropInterop = (dropZoneId, inputId) => {
    const dropZone = document.getElementById(dropZoneId);
    const fileInput = document.getElementById(inputId);

    if (!dropZone || !fileInput) return;

    // Handle dragover event - critical for signaling drop is allowed
    dropZone.addEventListener('dragover', (e) => {
        e.preventDefault();
        // Explicitly set dropEffect to 'copy' to indicate file can be dropped
        e.dataTransfer.dropEffect = 'copy';
        dropZone.classList.add('hover');
    });

    dropZone.addEventListener('dragleave', () => {
        dropZone.classList.remove('hover');
    });

    dropZone.addEventListener('drop', (e) => {
        e.preventDefault();
        dropZone.classList.remove('hover');

        if (e.dataTransfer.items) {

            [...e.dataTransfer.items].forEach((item, i) => {
                if (item.kind === "file") {
                    const file = item.getAsFile();
                    fileInput.file = item;
                }
            });
        }

        // Check for files in the dataTransfer object
        if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
            
            // Assign dropped files to the input
            fileInput.files = e.dataTransfer.files;

            // Trigger a change event so Blazor picks it up
            const event = new Event('change', { bubbles: true });
            fileInput.dispatchEvent(event);
        }
    });
};

window.dialogHelper = {
    showModal: (dialogElement) => {
        if (dialogElement?.showModal) {
            dialogElement.showModal();
        }
    },
    show: (dialogElement) => {
        if (dialogElement?.show) {
            dialogElement.show();
        }
    }
};

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

        tag.append(css);                         // append the whole batch once
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
        
        // Find existing rule for this class - handle media queries and complex selectors
        let existingRule = null;
        
        function findRuleInRuleList(rules) {
            for (let i = 0; i < rules.length; i++) {
                const rule = rules[i];
                
                // Handle media rules (nested rules within @media)
                if (rule.type === CSSRule.MEDIA_RULE && rule.cssRules) {
                    const nestedRule = findRuleInRuleList(rule.cssRules);
                    if (nestedRule) return nestedRule;
                }
                // Handle style rules
                else if (rule.type === CSSRule.STYLE_RULE && rule.selectorText === selector) {
                    return rule;
                }
            }
            return null;
        }
        
        existingRule = findRuleInRuleList(sheet.cssRules);
        
        if (existingRule) {
            // Add CSS properties to existing rule
            const properties = css.split(';').filter(prop => prop.trim());
            properties.forEach(prop => {
                const colonIndex = prop.indexOf(':');
                if (colonIndex > 0) {
                    const property = prop.substring(0, colonIndex).trim();
                    const value = prop.substring(colonIndex + 1).trim();
                    if (property && value) {
                        existingRule.style.setProperty(property, value);
                    }
                }
            });
        } else {
            // Create new CSS rule
            try {
                sheet.insertRule(`${selector}{${css}}`, sheet.cssRules.length);
            } catch (e) {
                // Fallback: append as text if insertRule fails
                tag.append(`${selector}{${css}}`);
            }
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

    clear(cls) {
        const tag = document.getElementById("rtb-styled");
        if (tag) this.removeRule(tag, '.' + cls);
    }
};