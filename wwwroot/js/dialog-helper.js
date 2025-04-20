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