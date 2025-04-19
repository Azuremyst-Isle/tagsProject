from flask import Flask, request, jsonify
from flask_sqlalchemy import SQLAlchemy
from datetime import datetime
from flask_cors import CORS

app = Flask(__name__)
CORS(app)

app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///rfid.db'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
db = SQLAlchemy(app)

class Item(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    rfid_tag = db.Column(db.String(120), unique=True, nullable=False)
    name = db.Column(db.String(120), nullable=False)
    description = db.Column(db.String(250))
    status = db.Column(db.String(50), default='available')
    last_updated = db.Column(db.DateTime, default=datetime.utcnow)

@app.route('/items', methods=['POST'])
def add_item():
    data = request.get_json()
    new_item = Item(
        rfid_tag=data['rfid_tag'],
        name=data['name'],
        description=data.get('description', '')
    )
    db.session.add(new_item)
    db.session.commit()
    return jsonify({'message': 'Item added successfully'}), 201

@app.route('/items', methods=['GET'])
def get_allitems():
    items = Item.query.all()
    output = []
    for item in items:
        item_data = {
            'rfid_tag': item.rfid_tag,
            'name': item.name,
            'description': item.description,
            'status': item.status,
            'last_updated': item.last_updated.isoformat()
        }
        output.append(item_data)
    return jsonify(output)

@app.route('/items/<rfid_tag>', methods=['GET'])
def get_item(rfid_tag):
    item = Item.query.filter_by(rfid_tag=rfid_tag).first()
    if item:
        return jsonify({
            'rfid_tag': item.rfid_tag,
            'name': item.name,
            'description': item.description,
            'status': item.status,
            'last_updated': item.last_updated.isoformat()
        })
    return jsonify({'message': 'Item not found'}), 404

@app.route('/items/<rfid_tag>', methods=['PUT'])
def update_item(rfid_tag):
    data = request.get_json()
    item = Item.query.filter_by(rfid_tag=rfid_tag).first()
    if item:
        item.name = data.get('name', item.name)
        item.description = data.get('description', item.description)
        item.status = data.get('status', item.status)
        item.last_updated = datetime.utcnow()
        db.session.commit()
        return jsonify({'message': 'Item updated successfully'})
    return jsonify({'message': 'Item not found'}), 404

@app.route('/items/<rfid_tag>', methods=['DELETE'])
def delete_item(rfid_tag):
    item = Item.query.filter_by(rfid_tag=rfid_tag).first()
    if item:
        db.session.delete(item)
        db.session.commit()
        return jsonify({'message': 'Item deleted successfully'})
    return jsonify({'message': 'Item not found'}), 404

if __name__ == '__main__':
    with app.app_context():
        db.create_all()
    app.run(debug=True)
