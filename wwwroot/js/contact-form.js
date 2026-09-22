(() => {
    "use strict";

    const form = document.getElementById("contactForm");

    if (!form) {
        return;
    }

    const feedback = document.getElementById("contactFormFeedback");
    const button = form.querySelector('button[type="submit"]');
    const originalButtonText = button.textContent.trim();

    let submitting = false;

    function showFeedback(message, success) {
        feedback.hidden = false;
        feedback.className = success
            ? "alert alert-success mb-4"
            : "alert alert-danger mb-4";

        feedback.style.whiteSpace = "pre-line";
        feedback.textContent = message;
        feedback.focus();
    }

    form.addEventListener("submit", async (event) => {
        event.preventDefault();

        if (submitting) {
            return;
        }

        submitting = true;
        button.disabled = true;
        button.textContent = form.dataset.sendingMessage;
        form.setAttribute("aria-busy", "true");
        feedback.hidden = true;

        try {
            const response = await fetch(form.action, {
                method: "POST",
                body: new FormData(form),
                credentials: "same-origin",
                headers: {
                    "Accept": "application/json"
                }
            });

            if (response.status === 429) {
                showFeedback(form.dataset.limitMessage, false);
                return;
            }

            const contentType = response.headers.get("content-type") ?? "";

            const result = contentType.includes("application/json")
                ? await response.json()
                : null;

            if (!response.ok || result?.success !== true) {
                const errors = Array.isArray(result?.errors)
                    ? result.errors.join("\n")
                    : "";

                showFeedback(
                    errors || result?.message || form.dataset.errorMessage,
                    false);

                return;
            }

            form.reset();
            showFeedback(form.dataset.successMessage, true);
        } catch {
            showFeedback(form.dataset.errorMessage, false);
        } finally {
            submitting = false;
            button.disabled = false;
            button.textContent = originalButtonText;
            form.removeAttribute("aria-busy");
        }
    });
})();