document.addEventListener("DOMContentLoaded", () => {
    const menu = document.getElementById("navbarCollapse");
    const navbar = document.querySelector(".nav-bar");
    const links = [...document.querySelectorAll("[data-section-link]")];

    const sections = links
        .map(link => ({
            link,
            section: document.getElementById(link.hash.slice(1))
        }))
        .filter(item => item.section);

    links.forEach(link => {
        link.addEventListener("click", () => {
            if (menu?.classList.contains("show")) {
                bootstrap.Collapse
                    .getOrCreateInstance(menu, { toggle: false })
                    .hide();
            }
        });
    });

    if (!sections.length) return;

    function updateActiveLink() {
        const offset = (navbar?.offsetHeight ?? 90) + 30;
        let current = sections[0];

        for (const item of sections) {
            if (item.section.getBoundingClientRect().top <= offset) {
                current = item;
            }
        }

        // Sayfanın sonunda footer bağlantısını da aktif yap.
        const atBottom =
            window.scrollY + window.innerHeight >=
            document.documentElement.scrollHeight - 2;

        if (atBottom) {
            current = sections[sections.length - 1];
        }

        sections.forEach(({ link }) => {
            const isActive = link === current.link;

            link.classList.toggle("active", isActive);

            if (isActive) {
                link.setAttribute("aria-current", "location");
            } else {
                link.removeAttribute("aria-current");
            }
        });
    }

    let scheduled = false;

    window.addEventListener("scroll", () => {
        if (scheduled) return;

        scheduled = true;

        requestAnimationFrame(() => {
            updateActiveLink();
            scheduled = false;
        });
    }, { passive: true });

    window.addEventListener("resize", updateActiveLink);
    window.addEventListener("load", updateActiveLink);

    updateActiveLink();
});

document.addEventListener("DOMContentLoaded", () => {
    const videoModal = document.getElementById("videoModal");
    const videoFrame = document.getElementById("videoFrame");
    const videoError = document.getElementById("videoError");

    if (videoModal && videoFrame && videoError) {
        videoModal.addEventListener("show.bs.modal", event => {
            const trigger = event.relatedTarget;
            const videoId = trigger?.getAttribute("data-video-id") ?? "";

            const isValid = /^[a-zA-Z0-9_-]{11}$/.test(videoId);

            videoError.classList.toggle("d-none", isValid);
            videoFrame.parentElement.classList.toggle("d-none", !isValid);

            videoFrame.removeAttribute("src");

            if (isValid) {
                videoFrame.src =
                    `https://www.youtube-nocookie.com/embed/${videoId}?rel=0`;
            }
        });

        videoModal.addEventListener("hide.bs.modal", () => {
            // Modal kapanırken video ve ses durur.
            videoFrame.removeAttribute("src");
        });
    }

    const serviceModal = document.getElementById("serviceModal");
    const serviceTitle = document.getElementById("serviceModalLabel");
    const serviceDescription =
        document.getElementById("serviceModalDescription");

    if (serviceModal && serviceTitle && serviceDescription) {
        serviceModal.addEventListener("show.bs.modal", event => {
            const trigger = event.relatedTarget;

            serviceTitle.textContent =
                trigger?.getAttribute("data-service-title")
                || "Hizmet Detayı";

            serviceDescription.textContent =
                trigger?.getAttribute("data-service-description")
                || "Bu hizmetin açıklaması henüz hazırlanmadı.";
        });
    }
});