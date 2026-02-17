// Utility: Replace underscores with spaces and capitalize first letter
function formatLabel(str) {
  if (!str) return "";
  const replaced = str.replace(/_/g, " ");
  return replaced.charAt(0).toUpperCase() + replaced.slice(1);
}
import { BACKEND_URL } from "./config.js";
const rootUrl = BACKEND_URL.root;
const demoPath = BACKEND_URL.demo;

const btn = document.getElementById("resetBtn");
// const box = document.getElementById("responseBox");
let detailsChart = null;
let ownerChart = null;
btn.addEventListener("click", async (e) => {
  e.preventDefault();
  console.log("Sending request...");
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
    console.log(`Status: ${res.status}\nMessage: ${data.message}`);
  } catch (err) {
    console.log("Error: " + err);
    console.error(err);
  }
});

// Summary
const ctx = document.getElementById("detailsChart");
const ctx2 = document.getElementById("ownerChart");
const summaryBtn = document.getElementById("summaryBtn");
summaryBtn.addEventListener("click", async (e) => {
  e.preventDefault();
  console.log("Sending request...");
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
      if (key.includes("owner")) {
        continue; // Skip owner-related keys for the bar chart
      }
      summaryLabels.push(formatLabel(key));
      summaryData.push(data[key]);
    }
    console.log(content);
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
            label: "Summary Data",
            data: summaryData,
            borderWidth: 1,
          },
        ],
      },
      options: {
        plugins: {
          legend: {
            display: false,
          },
        },
        responsive: true,
        scales: {
          y: {
            beginAtZero: true,
          },
        },
      },
    });
    ownerChart = new Chart(ctx2, {
      type: "pie",
      data: {
        labels: ["With Owner", "Without Owner"],
        datasets: [
          {
            label: "# of Items",
            data: [data.items_with_owner || 0, data.items_without_owner || 0],
            borderWidth: 1,
          },
        ],
      },
      options: {
        responsive: true,
        scales: {
          y: {
            beginAtZero: true,
          },
        },
      },
    });
  } catch (err) {
    console.log("Error: " + err);
    console.error(err);
  }
});

const runBtn = document.getElementById("runBtn");
runBtn.addEventListener("click", async (e) => {
  e.preventDefault();
  console.log("Sending request...");
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
    console.log(content);
  } catch (err) {
    console.log("Error: " + err);
    console.error(err);
  }
});

const smokeBtn = document.getElementById("smokeBtn");
smokeBtn.addEventListener("click", async (e) => {
  e.preventDefault();
  console.log("Sending request...");
  try {
    const res = await fetch(rootUrl + demoPath + "/smoke", {
      method: "GET",
    });
    const data = await res.json();
    let content = `Status: ${res.status}\n`;
    for (const key in data) {
      content += `${key}: ${data[key]}\n`;
    }
    console.log(content);
  } catch (err) {
    console.log("Error: " + err);
    console.error(err);
  }
});
