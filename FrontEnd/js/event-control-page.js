import { BACKEND_URL } from "./config.js";
const rootUrl = BACKEND_URL.root;
const demoPath = BACKEND_URL.demo;
const auditPath = BACKEND_URL.audit;
const apiPath = BACKEND_URL.items;

const box = document.getElementById("responseBox");

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
    const res = await fetch(rootUrl + apiPath + "/" + rfidTag + "/signal", {
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
