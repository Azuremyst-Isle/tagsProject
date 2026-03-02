import { BACKEND_URL } from "./config.js";
const rootUrl = BACKEND_URL.root;
const demoPath = BACKEND_URL.demo;
const auditPath = BACKEND_URL.audit;
const apiPath = BACKEND_URL.items;

const resultsDiv = document.getElementById("results");
const paginationDiv = document.getElementById("pagination");

const searchBtn = document.getElementById("searchBtn");
const searchInput = document.getElementById("searchInput");

let currentPage = 1;
let lastSearchTerm = "";
let totalPages = 1;
const PAGE_SIZE = 5;

function renderTable(items) {
  if (!items || items.length === 0) {
    resultsDiv.innerHTML = "<div>No results found.</div>";
    return;
  }
  let table = `<table class="search-table" style="width:100%; border-collapse:collapse;">`;
  table += `<thead><tr>
    <th>RFID Tag</th>
    <th>Name</th>
    <th>Description</th>
    <th>Status</th>
    <th>Last Updated</th>
    <th>Last Signal</th>
  </tr></thead><tbody>`;
  for (const item of items) {
    table += `<tr>
      <td>${item.rfid_tag ?? ""}</td>
      <td>${item.name ?? ""}</td>
      <td>${item.description ?? ""}</td>
      <td>${item.status ?? ""}</td>
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
  prevBtn.onclick = () => doSearch(lastSearchTerm, page - 1);
  const nextBtn = document.createElement("button");
  nextBtn.textContent = "Next";
  nextBtn.disabled = page >= totalPages;
  nextBtn.onclick = () => doSearch(lastSearchTerm, page + 1);
  const pageInfo = document.createElement("span");
  pageInfo.textContent = `Page ${page} of ${totalPages}`;
  paginationDiv.appendChild(prevBtn);
  paginationDiv.appendChild(pageInfo);
  paginationDiv.appendChild(nextBtn);
}

async function doSearch(term, page = 1) {
  lastSearchTerm = term;
  currentPage = page;
  resultsDiv.textContent = "Sending request...";
  paginationDiv.innerHTML = "";
  try {
    const res = await fetch(
      rootUrl +
        apiPath +
        `?search=${encodeURIComponent(term)}&sort_by=last_updated&sort_order=desc&status=available&page=${page}&page_size=${PAGE_SIZE}`,
      {
        method: "GET",
      },
    );
    const data = await res.json();
    renderTable(data.items);
    renderPagination(data.page, data.total, data.page_size);
  } catch (err) {
    resultsDiv.textContent = "Error: " + err;
    console.error(err);
  }
}

searchBtn.addEventListener("click", (e) => {
  e.preventDefault();
  const itemTerm = searchInput.value.trim();
  if (!itemTerm) {
    resultsDiv.textContent = "Please enter a search term.";
    paginationDiv.innerHTML = "";
    return;
  }
  doSearch(itemTerm, 1);
});

// Results will only be shown after clicking the search button.
