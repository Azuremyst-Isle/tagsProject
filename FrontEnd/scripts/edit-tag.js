import { BACKEND_URL } from "./config.js";

const rootUrl = BACKEND_URL.root;
const apiPath = BACKEND_URL.items;

document.addEventListener("DOMContentLoaded", async function () {
  const form = document.getElementById("editTagForm");
  const messageDiv = document.getElementById("formMessage");

  const params = new URLSearchParams(window.location.search);
  const rfidTag = params.get("rfid");

  if (!rfidTag) {
    messageDiv.textContent = "No RFID tag specified.";
    messageDiv.style.color = "red";
    if (form) form.style.display = "none";
    return;
  }

  // Pre-fill form with current item data
  try {
    const res = await fetch(
      `${rootUrl}${apiPath}/${encodeURIComponent(rfidTag)}`,
    );
    if (!res.ok) {
      messageDiv.textContent = "Failed to load item data.";
      messageDiv.style.color = "red";
      return;
    }
    const item = await res.json();
    form.rfid_tag.value = item.rfid_tag ?? "";
    form.name.value = item.name ?? "";
    form.description.value = item.description ?? "";
    form.status.value = item.status ?? "available";
    form.certification_code.value = item.certification_code ?? "";
    form.owner_name.value = item.owner_name ?? "";
  } catch (err) {
    messageDiv.textContent = `Network error loading item: ${err}`;
    messageDiv.style.color = "red";
    return;
  }

  // Form submission: send PUT request
  form.addEventListener("submit", async function (e) {
    e.preventDefault();
    const data = {
      name: form.name.value,
      description: form.description.value,
      status: form.status.value,
      certification_code: form.certification_code.value,
      owner_name: form.owner_name.value,
    };
    try {
      const res = await fetch(
        `${rootUrl}${apiPath}/${encodeURIComponent(rfidTag)}`,
        {
          method: "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(data),
        },
      );
      if (res.ok) {
        messageDiv.textContent = "Item updated successfully!";
        messageDiv.style.color = "green";
        setTimeout(() => {
          window.location.href = "item-control.html";
        }, 1000);
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
});
