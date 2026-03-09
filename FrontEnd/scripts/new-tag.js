import { BACKEND_URL } from "./config.js";
const rootUrl = BACKEND_URL.root;
// const BACKEND_URL = "http://localhost:5000";

// Form submission handler
document.addEventListener("DOMContentLoaded", function () {
  const form = document.getElementById("newTagForm");
  const messageDiv = document.getElementById("formMessage");
  if (form) {
    form.addEventListener("submit", async function (e) {
      e.preventDefault();
      const data = {
        rfid_tag: form.rfid_tag.value,
        name: form.name.value,
        description: form.description.value,
        status: form.status.value,
        certification_code: form.certification_code.value,
        owner_name: form.owner_name.value,
        retailer_id: parseInt(form.retailer_id.value, 10),
      };
      try {
        console.log("Submitting data:", data);
        console.log(`${rootUrl}/api/items`);
        const res = await fetch(`${rootUrl}/api/items`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(data),
        });
        if (res.ok) {
          messageDiv.textContent = "Item added successfully!";
          messageDiv.style.color = "green";
          form.reset();
        } else {
          const err = await res.text();
          messageDiv.textContent = `Error: ${err}`;
          messageDiv.style.color = "red";
        }
      } catch (error) {
        messageDiv.textContent = `Network error: ${error}`;
        messageDiv.style.color = "red";
      }
    });
  }
});
