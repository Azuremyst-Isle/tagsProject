export function formatLabel(str) {
  if (!str) return "";
  const replaced = str.replace(/_/g, " ");
  return replaced.charAt(0).toUpperCase() + replaced.slice(1);
}

export function createSortOptions(options) {
  let sortOptions = `<label for="sortOptions">Sort by:</label><select id="sortOptions">`;
  for (const option of options) {
    sortOptions += `<option value="${option}">${formatLabel(option)}</option>`;
  }
  sortOptions += `</select>`;
  // Add sort order select
  sortOptions += ` <label for="sortOrder">Order:</label><select id="sortOrder"><option value="asc">Ascending</option><option value="desc">Descending</option></select>`;
  return sortOptions;
}

export function renderPagination(
  page,
  total,
  pageSize,
  totalPages,
  paginationDiv,
  doSearch,
  lastSearchTerm,
) {
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
