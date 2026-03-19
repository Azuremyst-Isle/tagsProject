import { BACKEND_URL } from "./config.js";
import { formatLabel, createSortOptions } from "./utils.js";

const rootUrl = BACKEND_URL.root;
const apiPath = BACKEND_URL.items;

const resultsDiv = document.getElementById("results");
const paginationDiv = document.getElementById("pagination");

const PAGE_SIZE = 5;
let currentPage = 1;
let totalPages = 1;
let lastSortBy = "last_updated";
let lastSortOrder = "desc";
let itemsMap = {};

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

const displayHeaders = [
  "rfid_tag",
  "name",
  "status",
  "last_updated",
  "last_signal",
];

function renderTable(items) {
  if (!items || items.length === 0) {
    resultsDiv.innerHTML = "<div>No items found.</div>";
    return;
  }
  itemsMap = {};
  for (const item of items) {
    itemsMap[item.rfid_tag] = item;
  }
  const sortOptions = createSortOptions(displayHeaders);
  let table = `<table class="search-table">`;
  table += `<thead><tr>`;
  for (const header of displayHeaders) {
    table += `<th>${formatLabel(header)}</th>`;
  }
  table += `<th>Action</th>`;
  table += `</tr></thead><tbody>`;
  for (const item of items) {
    table += `<tr>
      <td>${item.rfid_tag ?? ""}</td>
      <td>${item.name ?? ""}</td>
      <td>${item.status ?? ""}</td>
      <td>${item.last_updated ? new Date(item.last_updated).toLocaleString() : ""}</td>
      <td>${item.last_signal ? new Date(item.last_signal).toLocaleString() : ""}</td>
      <td>
        <button class="view-btn" data-rfid="${item.rfid_tag}">View</button>
        <button class="delete-btn" data-rfid="${item.rfid_tag}">Delete</button>
      </td>
    </tr>`;
  }
  table += `</tbody></table>`;
  resultsDiv.innerHTML = sortOptions + table;

  // Add event listeners for sortOptions and sortOrder
  const sortSelect = document.getElementById("sortOptions");
  const orderSelect = document.getElementById("sortOrder");
  if (sortSelect) {
    sortSelect.value = lastSortBy;
    sortSelect.addEventListener("change", function () {
      lastSortBy = sortSelect.value;
      fetchItems(1, lastSortBy, lastSortOrder);
    });
  }
  if (orderSelect) {
    orderSelect.value = lastSortOrder;
    orderSelect.addEventListener("change", function () {
      lastSortOrder = orderSelect.value;
      fetchItems(1, lastSortBy, lastSortOrder);
    });
  }

  // Event delegation for delete and view buttons
  const tableElem = resultsDiv.querySelector("table");
  if (tableElem) {
    tableElem.addEventListener("click", function (e) {
      if (e.target && e.target.classList.contains("delete-btn")) {
        const rfid = e.target.getAttribute("data-rfid");
        deleteItem(rfid);
      }
      if (e.target && e.target.classList.contains("view-btn")) {
        const rfid = e.target.getAttribute("data-rfid");
        showItemModal(itemsMap[rfid]);
      }
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
  prevBtn.onclick = () => fetchItems(page - 1, lastSortBy, lastSortOrder);
  const nextBtn = document.createElement("button");
  nextBtn.textContent = "Next";
  nextBtn.disabled = page >= totalPages;
  nextBtn.onclick = () => fetchItems(page + 1, lastSortBy, lastSortOrder);
  const pageInfo = document.createElement("span");
  pageInfo.textContent = `Page ${page} of ${totalPages}`;
  paginationDiv.appendChild(prevBtn);
  paginationDiv.appendChild(pageInfo);
  paginationDiv.appendChild(nextBtn);
}

async function fetchItems(
  page = 1,
  sortBy = lastSortBy,
  sortOrder = lastSortOrder,
) {
  resultsDiv.textContent = "Loading...";
  paginationDiv.innerHTML = "";
  try {
    const res = await fetch(
      `${rootUrl}${apiPath}?page=${page}&page_size=${PAGE_SIZE}&sort_by=${encodeURIComponent(sortBy)}&sort_order=${encodeURIComponent(sortOrder)}`,
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

async function deleteItem(rfidTag) {
  if (
    !confirm(`Are you sure you want to delete item with RFID tag ${rfidTag}?`)
  ) {
    return;
  }
  // Implement the actual deletion logic here
  try {
    const res = await fetch(
      `${rootUrl}${apiPath}/${encodeURIComponent(rfidTag)}`,
      { method: "DELETE" },
    );
    if (res.ok) {
      alert(`Item with RFID tag ${rfidTag} deleted successfully.`);
      fetchItems(currentPage, lastSortBy, lastSortOrder); // Refresh the list
    } else {
      const errorData = await res.json();
      alert(`Failed to delete item: ${errorData.message || res.statusText}`);
    }
  } catch (err) {
    resultsDiv.textContent = "Error: " + err;
    console.error(err);
  }
}

function injectModal() {
  if (document.getElementById("itemModal")) return;
  const modal = document.createElement("div");
  modal.id = "itemModal";
  modal.className = "modal-overlay";
  modal.style.display = "none";
  modal.innerHTML = `
    <div class="modal-card card">
      <button class="modal-close" id="modalCloseBtn">&times;</button>
      <h2>Item Details</h2>
      <div class="modal-fields" id="modalFields"></div>
    </div>
  `;
  document.body.appendChild(modal);
  document
    .getElementById("modalCloseBtn")
    .addEventListener("click", closeModal);
  modal.addEventListener("click", function (e) {
    if (e.target === modal) closeModal();
  });
  document.addEventListener("keydown", function (e) {
    if (e.key === "Escape") closeModal();
  });
}

function closeModal() {
  const modal = document.getElementById("itemModal");
  if (modal) modal.style.display = "none";
}

function showItemModal(item) {
  const modal = document.getElementById("itemModal");
  if (!modal) return;
  const fieldsDiv = document.getElementById("modalFields");
  let html = "";
  for (const key of tableHeaders) {
    let value = item[key] ?? "";
    if ((key === "last_updated" || key === "last_signal") && value) {
      value = new Date(value).toLocaleString();
    }
    html += `
      <div class="modal-field">
        <label>${formatLabel(key)}</label>
        <span>${value}</span>
      </div>`;
  }
  html += `
    <div class="modal-actions">
      <a class="edit-btn" href="edit-tag.html?rfid=${encodeURIComponent(item.rfid_tag)}">Edit</a>
    </div>`;
  fieldsDiv.innerHTML = html;
  modal.style.display = "flex";
}

window.addEventListener("DOMContentLoaded", function () {
  injectModal();
  fetchItems(1);
});
