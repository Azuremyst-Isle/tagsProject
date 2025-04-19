# RFID Object Tracking System – Update 1

This project is a lightweight web-based system for identifying and tracking high-value physical objects using RFID/NFC adhesive tags. It consists of a minimal backend API and a simple frontend interface that work together to link a physical item (via a tag) to a digital record.

---

## 🔧 Features

- Add, retrieve, and update object data via RESTful API
- Interface with RFID/NFC tags (e.g. NTAG213)
- Store owner info, object condition, and third-party certification codes (PSA/BGS)
- Web frontend for manual interaction

---

## 📦 Project Structure

```
update_1/
├── app.py          # Flask backend API
├── index.html      # Web frontend UI
├── README.md       # Project documentation
├── test_app.py     # (optional) API unit tests
```

---

## ▶️ Quick Start

### 1. Requirements
- Python 3.7+
- Flask
- flask_sqlalchemy
- flask_cors

### 2. Install dependencies
```bash
pip install flask flask_sqlalchemy flask_cors
```

### 3. Run the backend
```bash
python app.py
```

The backend will be running on `http://localhost:5000`

### 4. Open the frontend
Open `index.html` in your browser.

---

## 🧠 Data Model
Each object has the following fields:
- `rfid_tag`: Unique alphanumeric ID from the NFC/RFID tag (primary key)
- `name`: Name of the item
- `description`: Optional description
- `status`: Physical condition (e.g. Excellent, Fair)
- `owner`: Owner's name or identifier
- `cert_code`: Optional third-party certification ID (e.g. PSA or BGS code)
- `last_updated`: Timestamp

---

## 🌐 API Overview

### Base URL
```
http://localhost:5000
```

### Endpoints
| Endpoint            | Method | Description                  |
|---------------------|--------|------------------------------|
| `/items`            | POST   | Add a new item               |
| `/items/<rfid_tag>` | GET    | Retrieve item by tag         |
| `/items/<rfid_tag>` | PUT    | Update existing item         |

See `docs/api.md` or Postman collection for payloads and responses.

---

## 🏷️ Hardware Compatibility
This system is designed to work with:
- NFC tags (e.g. NTAG213, readable via smartphone)
- Passive UHF tags (via USB reader, optional)

No hardware interaction is required yet. UID values from tags are entered manually or scanned via NFC.

---

## 📌 Roadmap
- [ ] Add user authentication
- [ ] Mobile-friendly frontend
- [ ] NFC tag writing / encoding tool
- [ ] Optional geolocation and tamper detection (active tags)

---

## 🤝 Credits
Francesco (Glasgow, Lecce), Tony (Puerto Progreso, Mexico), Enrico (Lecce)
