const apiBase = "http://localhost:5000";

function addItem() {
  fetch(apiBase + "/api/items", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      rfid_tag: document.getElementById("rfid_tag").value,
      name: document.getElementById("name").value,
      owner_name: document.getElementById("owner_name").value,
      certification_code: document.getElementById("certification_code").value,
      description: document.getElementById("description").value,
    }),
  })
    .then((response) => response.json())
    .then(
      () => (window.location.href = "index.html") // Redirect to home page after adding an item
    );
}

function getItem() {
  document.getElementById("resultTable").style.visibility = "visible";
  const tag = document.getElementById("search_tag").value;
  fetch(apiBase + `/api/items/${tag}`)
    .then((response) => {
      if (response.status === 404) {
        return null; // Handle 404 case
      }
      return response.json();
    })
    .then((data) => {
      const tableBody = document.getElementById("itemTableBody");
      tableBody.innerHTML = ""; // Clear previous results
      if (data) {
        const formattedDate = new Date(data.last_updated).toLocaleString(
          "en-GB",
          {
            hour: "2-digit",
            minute: "2-digit",
            second: "2-digit",
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
          }
        );
        const row = `<tr>
                    <td>${data.rfid_tag}</td>
                    <td>${data.name}</td>
                    <td>${data.description}</td>
                    <td>${data.status}</td>
                    <td>${data.owner_name}</td>
                    <td>${data.certification_code}</td>
                    <td>${formattedDate}</td>
                    <td><button class="btn btn-danger" onclick="deleteItem('${data.rfid_tag}')">Delete</button></td>
                </tr>`;
        tableBody.innerHTML = row;
      } else {
        tableBody.innerHTML = '<tr><td colspan="5">No data found</td></tr>';
      }
    });
}

function getAllItems() {
  fetch(apiBase + "/api/items")
    .then((response) => response.json())
    .then((data) => {
      const tableBody = document.getElementById("itemTableBody");
      tableBody.innerHTML = ""; // Clear previous results
      data.forEach((item) => {
        const row = `<tr>
                    <td>${item.rfid_tag}</td>
                    <td>${item.name}</td>
                    <td>${item.description}</td>
                    <td>${item.status}</td>
                    <td>${item.owner_name}</td>
                    <td>${item.certification_code}</td>
                </tr>`;
        tableBody.innerHTML += row;
      });
    });
}

function deleteItem(rfidTag) {
  fetch(apiBase + `/api/items/${rfidTag}`, {
    method: "DELETE",
  }).then((response) => {
    if (response.ok) {
      alert("Item deleted successfully");
      window.location.reload(); // Refresh the page after alert is closed
    } else {
      alert("Item not found");
    }
  });
}
