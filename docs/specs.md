1. What the code does (and how it works)

We’re building a lightweight tracking system for high-value objects using RFID tags (the kind you can stick on something and scan). The goal is to let users scan a tag and immediately see who owns the item, its description, and other identifying info.

Here’s the setup:

- `app.py`: This is the backend. It's a Flask-based API that talks to a small local SQLite database. It can:
  - Create new items linked to a unique RFID tag (`POST /items`)
  - Fetch item data by tag (`GET /items/<rfid_tag>`)
  - Update item info (`PUT /items/<rfid_tag>`)

- `index.html`: The frontend web interface. It runs in a browser and lets the user:
  - Add new RFID-linked items
  - Search for existing ones by tag

All communication between the frontend and backend happens through REST API calls.

This version is called "update 1". It’s fully functional as a proof of concept.

---

2. Project goals: what we're trying to do, and why

We want to build a simple and secure way to identify and validate ownership of valuable physical items. Think collector's items, safes, containers, vehicles, rare books, etc.

Why?
- Fraud is common in private sales especially for collectibles.
- People want proof that what they’re buying is real and belongs to the seller.

How?
- By attaching a smart tag (RFID or NFC) to the item.
- When scanned, that tag reveals secure info (via a database): owner, condition, notes, value, etc.
- Buyers or anyone with the tag can verify its legitimacy before buying or transferring it.

---

3. The role of the sticker

The RFID/NFC sticker is the heart of the system. It's like a license plate for the object:
- It stores a unique ID (the `rfid_tag` value in the database).
- That ID is what ties the physical object to its digital record.
- Anyone scanning it can retrieve and view that data from the backend.

For now, we’re not storing data directly on the tag – the tag is just a pointer to data in the database.

---

4. What tag you’ll receive

You’ll receive a NTAG213 NFC adhesive label. It looks like a thin circular or rectangular sticker and can be scanned with any NFC-compatible smartphone.

It has:
- A unique ID hardcoded into it.
- Optional memory if needed (not used right now).

You can scan it and use the UID value as input to test the code.

---

5. Minimum viable product (MVP): what it must do to be test-ready

For now, the system only needs to do this:
- Let us scan a tag and see the data linked to it.
- Allow adding and updating that data (object description, owner info, etc.).
- Work on a local machine (backend + frontend).

No login, no blockchain, no fancy encryption yet. Just core functionality.

---

6. Future features: geolocation and alerts

Later versions might include:
- GPS tracking of the object (active tag with battery)
- Tamper or motion detection, with alerts sent to a mobile device
- Possibly a mobile app frontend instead of a browser one

But for now, the focus is just on getting this core infrastructure working and testable.