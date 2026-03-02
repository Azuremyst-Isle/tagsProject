import { BACKEND_URL } from "./config.js";
const rootUrl = BACKEND_URL.root;
const demoPath = BACKEND_URL.demo;
const auditPath = BACKEND_URL.audit;
const apiPath = BACKEND_URL.items;

const box = document.getElementById("responseBox");

const searchBtn = document.getElementById("searchBtn");
const searchInput = document.getElementById("searchInput");
searchBtn.addEventListener("click", async (e) => {
  e.preventDefault();
  const itemTerm = searchInput.value.trim();
  if (!itemTerm) {
    box.textContent = "Please enter a search term.";
    return;
  }
  box.textContent = "Sending request...";
  try {
    const res = await fetch(
      rootUrl +
        apiPath +
        "?search=" +
        itemTerm +
        "&sort_by=last_updated&sort_order=desc&status=available&page=1&page_size=5",
      {
        method: "GET",
      },
    );
    const data = await res.json();
    let content = `Status: ${res.status}\n`;
    for (const key in data) {
      const value =
        typeof data[key] === "object" ? JSON.stringify(data[key]) : data[key];
      content += `${key}: ${value}\n`;
    }
    box.textContent = content;
  } catch (err) {
    box.textContent = "Error: " + err;
    console.error(err);
  }
});
