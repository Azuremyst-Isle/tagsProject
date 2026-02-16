import { BACKEND_URL } from "./config.js";
const rootUrl = BACKEND_URL.root;
const demoPath = BACKEND_URL.demo;
const auditPath = BACKEND_URL.audit;
const apiPath = BACKEND_URL.items;

const btn = document.getElementById("resetBtn");
const box = document.getElementById("responseBox");
let detailsChart = null;
let ownerChart = null;
btn.addEventListener("click", async (e) => {
  e.preventDefault();
  box.textContent = "Sending request...";
  // Destroy chart if it exists
  if (detailsChart) {
    detailsChart.destroy();
    detailsChart = null;
  }
  if (ownerChart) {
    ownerChart.destroy();
    ownerChart = null;
  }
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

// Summary
const ctx = document.getElementById("detailsChart");
const ctx2 = document.getElementById("ownerChart");
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
    let summaryLabels = [];
    let summaryData = [];
    for (const key in data) {
      content += `${key}: ${data[key]}\n`;
      summaryLabels.push(key);
      summaryData.push(data[key]);
    }
    box.textContent = content;
    // Update Chart
    // Chart.js example
    if (detailsChart) {
      detailsChart.destroy();
      detailsChart = null;
    }
    if (ownerChart) {
      ownerChart.destroy();
      ownerChart = null;
    }
    detailsChart = new Chart(ctx, {
      type: "bar",
      data: {
        labels: summaryLabels,
        datasets: [
          {
            label: "# of Votes",
            data: summaryData,
            borderWidth: 1,
          },
        ],
      },
      options: {
        scales: {
          y: {
            beginAtZero: true,
          },
        },
      },
    });
    ownerChart = new Chart(ctx2, {
      type: "bar",
      data: {
        labels: ["Red", "Blue", "Yellow", "Green", "Purple", "Orange"],
        datasets: [
          {
            label: "# of Votes",
            data: [12, 19, 3, 5, 2, 3],
            borderWidth: 1,
          },
        ],
      },
      options: {
        scales: {
          y: {
            beginAtZero: true,
          },
        },
      },
    });
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
