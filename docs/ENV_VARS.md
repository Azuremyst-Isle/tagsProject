# ⚙️ Environment Variables – Backend Configuration

This file documents the environment variables used to configure the Flask backend.

---

## 📄 `.env` file structure (example)

```env
FLASK_APP=app.py
FLASK_ENV=development
DATABASE_URL=sqlite:///rfid.db
SECRET_KEY=supersecuredevkey
API_AUTH_TOKEN=devtoken12345
```

---

## 🔍 Description of Variables

| Variable       | Required | Default               | Description                                                        |
|----------------|----------|-----------------------|--------------------------------------------------------------------|
| `FLASK_APP`    | ✅       | `app.py`              | Entry point for Flask app                                          |
| `FLASK_ENV`    | ✅       | `development`         | Controls debug mode                                                |
| `DATABASE_URL` | ✅       | `sqlite:///rfid.db`   | SQLAlchemy DB URI (can point to PostgreSQL, etc.)                 |
| `SECRET_KEY`   | ✅       | -                     | Flask session and security secret                                 |
| `API_AUTH_TOKEN`| ❗Optional| -                    | Token for basic header auth (used during testing)                  |

---

## 🛠 Notes

- Use `python-dotenv` to load the `.env` file into Flask (`from dotenv import load_dotenv`)
- Do **not** commit `.env` to GitHub. Use `.gitignore`.
- For production, use services like **Render**, **Railway**, or **Fly.io** which allow setting environment variables via their UI.

---
