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