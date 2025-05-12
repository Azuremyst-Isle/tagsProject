# 🚀 DEPLOYMENT OPTIONS – Web App + API

This document outlines possible deployment strategies for our system.

## Goal
Deploy the web interface (`index.html`) and the Flask backend (`app.py`) so that all three collaborators can test the system remotely in a shared environment.

---

## 🌐 Web Frontend (Static Hosting)

| Option       | Supports Index.html | Free Tier | Notes                                   |
|--------------|---------------------|-----------|-----------------------------------------|
| GitHub Pages | ✅ Yes              | ✅ Yes    | Static files only (no backend support)  |
| Netlify      | ✅ Yes              | ✅ Yes    | Drag-and-drop deploy                    |
| Vercel       | ✅ Yes              | ✅ Yes    | Well-integrated with GitHub             |

---

## 🧠 Flask Backend (Dynamic API Hosting)

| Option       | Supports Flask | Free Tier | Notes                                     |
|--------------|----------------|-----------|-------------------------------------------|
| Render.com   | ✅ Yes         | ✅ Yes    | Easiest deploy, works well for Flask      |
| Railway.app  | ✅ Yes         | ✅ Yes    | Requires GitHub auth                      |
| Fly.io       | ✅ Yes         | ✅ Yes    | Lightweight, supports SQLite              |

---

## Recommendation for Now

- Host the `index.html` on **GitHub Pages** (free and simple)
- Host the API on **Render.com** or **Railway.app** for early-stage collaborative testing
- Use a `.env` file or config settings to disable authentication during early phases, but keep endpoints private

---

## Next Step
Implement a CI step to auto-deploy from GitHub to both frontend and backend environments with minimal manual work.
