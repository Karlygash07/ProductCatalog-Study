(() => {
    const header = document.querySelector('.masthead');
    const updateHeader = () => header?.classList.toggle('scrolled', window.scrollY > 12);
    updateHeader();
    window.addEventListener('scroll', updateHeader, { passive: true });

    const form = document.querySelector('.catalog-filter');
    form?.querySelectorAll('[data-auto-filter]').forEach(control => {
        control.addEventListener('change', () => form.requestSubmit());
    });

    const imageUrlInput = document.querySelector('#ImageUrl');
    const previewBox = document.querySelector('#urlPhotoPreview');
    const previewImage = document.querySelector('#urlPhotoPreviewImage');
    const updateImagePreview = () => {
        const value = imageUrlInput?.value.trim();
        if (!value || !previewBox || !previewImage) return;
        previewImage.src = value;
        previewBox.hidden = false;
    };
    imageUrlInput?.addEventListener('input', updateImagePreview);
    imageUrlInput?.addEventListener('change', updateImagePreview);
    previewImage?.addEventListener('error', () => { if (previewBox) previewBox.hidden = true; });
    updateImagePreview();

    const nav = document.querySelector('#siteNav');
    nav?.querySelectorAll('a').forEach(link => link.addEventListener('click', () => {
        if (window.bootstrap && nav.classList.contains('show')) {
            window.bootstrap.Collapse.getOrCreateInstance(nav).hide();
        }
    }));
})();
