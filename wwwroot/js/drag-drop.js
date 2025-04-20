window.initDragDropInterop = (dropZoneId, inputId) => {
    const dropZone = document.getElementById(dropZoneId);
    const fileInput = document.getElementById(inputId);

    if (!dropZone || !fileInput) return;

    dropZone.addEventListener('dragover', (e) => {
        e.preventDefault();
        dropZone.classList.add('hover');
    });

    dropZone.addEventListener('dragleave', () => {
        dropZone.classList.remove('hover');
    });

    dropZone.addEventListener('drop', (e) => {
        e.preventDefault();
        dropZone.classList.remove('hover');

        if (e.dataTransfer.files.length > 0) {
            // Assign dropped files to the input
            fileInput.files = e.dataTransfer.files;

            // Trigger a change event so Blazor picks it up
            const event = new Event('change', { bubbles: true });
            fileInput.dispatchEvent(event);
        }
    });
};
