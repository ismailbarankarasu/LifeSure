(() => {
    "use strict";

    const modal = document.getElementById("serviceDetailsModal");

    if (!modal) {
        return;
    }

    const title = modal.querySelector("#serviceDetailsModalLabel");
    const description = modal.querySelector("#serviceDetailsDescription");

    modal.addEventListener("show.bs.modal", (event) => {
        const trigger = event.relatedTarget;

        if (!(trigger instanceof HTMLElement)) {
            return;
        }

        title.textContent = trigger.dataset.serviceTitle ?? "";
        description.textContent = trigger.dataset.serviceDescription ?? "";
    });

    modal.addEventListener("hidden.bs.modal", () => {
        title.textContent = "";
        description.textContent = "";
    });
})();