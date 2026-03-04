import { BACKEND_URL } from "./config.js";
import { formatLabel } from "./utils.js";
const rootUrl = BACKEND_URL.root;
const apiPath = BACKEND_URL.items;

const resultsDiv = document.getElementById("results");
const paginationDiv = document.getElementById("pagination");

const searchBtn = document.getElementById("searchBtn");
const searchInput = document.getElementById("searchInput");

let currentPage = 1;
let lastSearchTerm = "";
let lastSortBy = "last_updated";
let lastSortOrder = "desc";
let totalPages = 1;
const PAGE_SIZE = 5;

const tableHeaders = [
  "rfid_tag",
  "name",
  "description",
  "status",
  "last_updated",
  "last_signal",
];

function createSortOptions(options) {
  let sortOptions = `<label for="sortOptions">Sort by:</label><select id="sortOptions">`;
  for (const option of options) {
    sortOptions += `<option value="${option}">${formatLabel(option)}</option>`;
  }
  sortOptions += `</select>`;
  return sortOptions;
}

function renderTable(items) {
  if (!items || items.length === 0) {
    resultsDiv.innerHTML = "<div>No results found.</div>";
    return;
  }
  const sortOptions = createSortOptions(tableHeaders);
  let table = `<table class="search-table" style="">`;
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
      <td>${item.last_updated ? new Date(item.last_updated).toLocaleString() : ""}</td>
      <td>${item.last_signal ? new Date(item.last_signal).toLocaleString() : ""}</td>
    </tr>`;
  }
  table += `</tbody></table>`;
  resultsDiv.innerHTML = sortOptions + table;

  // Add event listener for sortOptions
  const sortSelect = document.getElementById("sortOptions");
  if (sortSelect) {
    sortSelect.value = lastSortBy;
    sortSelect.addEventListener("change", function () {
      lastSortBy = sortSelect.value;
      // Toggle sort order if same field, else reset to desc
      if (lastSortBy === lastSortBy) {
        lastSortOrder = lastSortOrder === "desc" ? "asc" : "desc";
      } else {
        lastSortOrder = "desc";
      }
      doSearch(lastSearchTerm, 1, lastSortBy, lastSortOrder);
    });
  }
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

async function doSearch(
  term,
  page = 1,
  sortBy = lastSortBy,
  sortOrder = lastSortOrder,
) {
  lastSearchTerm = term;
  currentPage = page;
  lastSortBy = sortBy;
  lastSortOrder = sortOrder;
  const statusFilter = document.getElementById("options").value;
  let statusQuery = statusFilter == "Any" ? "" : `status=${statusFilter}`;
  resultsDiv.textContent = "Sending request...";
  paginationDiv.innerHTML = "";
  try {
    const res = await fetch(
      rootUrl +
        apiPath +
        `?search=${encodeURIComponent(term)}&sort_by=${encodeURIComponent(sortBy)}&sort_order=${encodeURIComponent(sortOrder)}&${statusQuery}&page=${page}&page_size=${PAGE_SIZE}`,
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
  doSearch(itemTerm, 1, lastSortBy, lastSortOrder);
});

// Dynamically resize select based on selected option
window.addEventListener("DOMContentLoaded", function () {
  const select = document.getElementById("options");
  function resizeSelect() {
    if (!select) return;
    const temp = document.createElement("span");
    temp.style.visibility = "hidden";
    temp.style.position = "fixed";
    temp.style.font = window.getComputedStyle(select).font;
    temp.style.padding = window.getComputedStyle(select).padding;
    temp.textContent = select.options[select.selectedIndex].text;
    document.body.appendChild(temp);
    // Add some extra space for the dropdown arrow
    select.style.width = temp.offsetWidth + 36 + "px";
    document.body.removeChild(temp);
  }
  if (select) {
    resizeSelect();
    select.addEventListener("change", resizeSelect);
  }
});
