import { BACKEND_URL } from "./config.js";
import { formatLabel } from "./utils.js";

const rootUrl = BACKEND_URL.root;
const apiPath = BACKEND_URL.items;

const resultsDiv = document.getElementById("results");
const paginationDiv = document.getElementById("pagination");

const PAGE_SIZE = 5;
let currentPage = 1;
let totalPages = 1;

const tableHeaders = [
  "rfid_tag",
  "name",
  "description",
  "status",
  "certification_code",
  "owner_name",
  "owner_email",
  "last_updated",
  "last_signal",
];

function renderTable(items) {
  if (!items || items.length === 0) {
    resultsDiv.innerHTML = "<div>No items found.</div>";
    return;
  }
  let table = `<table class="search-table">`;
  table += `<thead><tr>`;
  for (const header of tableHeaders) {
    table += `<th>${formatLabel(header)}</th>`;
  }
  table += `</tr></thead><tbody>`;
  for (const item of items) {
    table += `<tr>
      <td>${item.rfid_tag ?? ""}</td>
      <td>${item.name ?? ""}</td>
      <td>${item.description ?? ""}</td>
      <td>${item.status ?? ""}</td>
      <td>${item.certification_code ?? ""}</td>
      <td>${item.owner_name ?? ""}</td>
      <td>${item.owner_email ?? ""}</td>
      <td>${item.last_updated ? new Date(item.last_updated).toLocaleString() : ""}</td>
      <td>${item.last_signal ? new Date(item.last_signal).toLocaleString() : ""}</td>
    </tr>`;
  }
  table += `</tbody></table>`;
  resultsDiv.innerHTML = table;
}

function renderPagination(page, total, pageSize) {
  paginationDiv.innerHTML = "";
  totalPages = Math.ceil(total / pageSize);
  if (totalPages <= 1) return;
  const prevBtn = document.createElement("button");
  prevBtn.textContent = "Previous";
  prevBtn.disabled = page <= 1;
  prevBtn.onclick = () => fetchItems(page - 1);
  const nextBtn = document.createElement("button");
  nextBtn.textContent = "Next";
  nextBtn.disabled = page >= totalPages;
  nextBtn.onclick = () => fetchItems(page + 1);
  const pageInfo = document.createElement("span");
  pageInfo.textContent = `Page ${page} of ${totalPages}`;
  paginationDiv.appendChild(prevBtn);
  paginationDiv.appendChild(pageInfo);
  paginationDiv.appendChild(nextBtn);
}

async function fetchItems(page = 1) {
  resultsDiv.textContent = "Loading...";
  paginationDiv.innerHTML = "";
  try {
    const res = await fetch(
      `${rootUrl}${apiPath}?page=${page}&page_size=${PAGE_SIZE}`,
      { method: "GET" },
    );
    const data = await res.json();
    renderTable(data.items);
    renderPagination(data.page, data.total, data.page_size);
    currentPage = data.page;
  } catch (err) {
    resultsDiv.textContent = "Error: " + err;
    console.error(err);
  }
}

window.addEventListener("DOMContentLoaded", function () {
  fetchItems(1);
});
