const rootUrl = "http://localhost:5000";
const demoPath = "/api/demo";
const auditPath = "/api/audit/items";
const apiPath = "/api/items";

const btn = document.getElementById("resetBtn");
const box = document.getElementById("responseBox");
btn.addEventListener("click", async (e) => {
  e.preventDefault();
  box.textContent = "Sending request...";
  try {
    const res = await fetch(rootUrl + demoPath + "/reset", {
      method: "POST",
    });
    const data = await res.json();
    box.textContent = `Status: ${res.status}\nMessage: ${data.message}`;
  } catch (err) {
    box.textContent = "Error: " + err;
    console.error(err);
  }
});

const summaryBtn = document.getElementById("summaryBtn");
summaryBtn.addEventListener("click", async (e) => {
  e.preventDefault();
  box.textContent = "Sending request...";
  try {
    const res = await fetch(rootUrl + demoPath + "/summary", {
      method: "GET",
    });
    const data = await res.json();
    let content = `Status: ${res.status}\n`;
    for (const key in data) {
      content += `${key}: ${data[key]}\n`;
    }
    box.textContent = content;
  } catch (err) {
    box.textContent = "Error: " + err;
    console.error(err);
  }
});

const signalBtn = document.getElementById("signalBtn");
const rfidInput = document.getElementById("rfidInput");
signalBtn.addEventListener("click", async (e) => {
  e.preventDefault();
  const rfidTag = rfidInput.value.trim();
  if (!rfidTag) {
    box.textContent = "Please enter an RFID Tag.";
    return;
  }
  box.textContent = "Sending request...";
  try {
    const res = await fetch(rootUrl + "/api/items/" + rfidTag + "/signal", {
      method: "POST",
    });
    const data = await res.json();
    let content = `Status: ${res.status}\n`;
    for (const key in data) {
      content += `${key}: ${data[key]}\n`;
    }
    box.textContent = content;
  } catch (err) {
    box.textContent = "Error: " + err;
    console.error(err);
  }
});

const synthBtn = document.getElementById("synthBtn");
synthBtn.addEventListener("click", async (e) => {
  e.preventDefault();
  const rfidTag = rfidInput.value.trim();
  if (!rfidTag) {
    box.textContent = "Please enter an RFID Tag.";
    return;
  }
  box.textContent = "Sending request...";
  try {
    const res = await fetch(
      rootUrl + demoPath + "/fill-events/" + rfidTag + "?number=5",
      {
        method: "POST",
      },
    );
    const data = await res.json();
    let content = `Status: ${res.status}\n`;
    for (const key in data) {
      content += `${key}: ${data[key]}\n`;
    }
    box.textContent = content;
  } catch (err) {
    box.textContent = "Error: " + err;
    console.error(err);
  }
});

const auditBtn = document.getElementById("auditBtn");
auditBtn.addEventListener("click", async (e) => {
  e.preventDefault();
  const rfidTag = rfidInput.value.trim();
  if (!rfidTag) {
    box.textContent = "Please enter an RFID Tag.";
    return;
  }
  box.textContent = "Sending request...";
  try {
    const res = await fetch(rootUrl + auditPath + "/" + rfidTag, {
      method: "GET",
    });
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

const downloadBtn = document.getElementById("downloadBtn");
downloadBtn.addEventListener("click", async (e) => {
  e.preventDefault();
  const rfidTag = rfidInput.value.trim();
  if (!rfidTag) {
    box.textContent = "Please enter an RFID Tag.";
    return;
  }
  box.textContent = "Sending request...";
  try {
    const res = await fetch(rootUrl + auditPath + "/" + rfidTag + "/csv", {
      method: "GET",
    });
    const data = await res.text();
    const blob = new Blob([data], { type: "text/csv" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `${rfidTag}_audit.csv`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
    box.textContent = "Download initiated.";
  } catch (err) {
    box.textContent = "Error: " + err;
    console.error(err);
  }
});

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

const runBtn = document.getElementById("runBtn");
runBtn.addEventListener("click", async (e) => {
  e.preventDefault();
  box.textContent = "Sending request...";
  try {
    const res = await fetch(rootUrl + demoPath + "/runbook", {
      method: "POST",
    });
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

const smokeBtn = document.getElementById("smokeBtn");
smokeBtn.addEventListener("click", async (e) => {
  e.preventDefault();
  box.textContent = "Sending request...";
  try {
    const res = await fetch(rootUrl + demoPath + "/smoke", {
      method: "GET",
    });
    const data = await res.json();
    let content = `Status: ${res.status}\n`;
    for (const key in data) {
      content += `${key}: ${data[key]}\n`;
    }
    box.textContent = content;
  } catch (err) {
    box.textContent = "Error: " + err;
    console.error(err);
  }
});
