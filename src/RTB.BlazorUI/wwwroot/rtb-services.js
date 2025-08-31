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

window.inputService = {
    register: function (dotnetRef) {
        function down(e) {
            dotnetRef.invokeMethodAsync("OnKeyDown", e.key);
        }
        function up(e) {
            dotnetRef.invokeMethodAsync("OnKeyUp", e.key);
        }

        document.addEventListener("keydown", down);
        document.addEventListener("keyup", up);

        this._down = down;
        this._up = up;
    },

    unregister: function () {
        if (this._down) document.removeEventListener("keydown", this._down);
        if (this._up) document.removeEventListener("keyup", this._up);
    }
};

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

window.popoverHelper = {
    show: (popoverId) => {
        const popoverElement = document.getElementById(popoverId);
        if (popoverElement?.showPopover) {
            popoverElement.showPopover();
        }
    },
    close: (popoverId) => {
        const popoverElement = document.getElementById(popoverId);
        if (popoverElement?.hidePopover) {
            popoverElement.hidePopover(); // same as 'hide'
        }
    },
    toggle: (popoverId) => {
        const popoverElement = document.getElementById(popoverId);
        if (popoverElement?.togglePopover) {
            popoverElement.togglePopover();
        }
    }
}