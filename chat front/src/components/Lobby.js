import { useState } from 'react';
import { Form, Button } from 'react-bootstrap';

const Lobby = ({ joinRoom }) => {
    const [user, setUser] = useState();
    const [room, setRoom] = useState();

    return <Form className='lobby'
        onSubmit={e => {
            e.preventDefault();
            fetch('https://localhost:44382/api/Rooom/'+room, {
                method: 'POST'
                // headers: { 'Content-Type': 'application/json' },
                // body: JSON.stringify({ room: room })
            });
            joinRoom(user, room);

        }} >
        <Form.Group>
            <Form.Control placeholder="name" onChange={e => setUser(e.target.value)} />
            <Form.Control placeholder="room" onChange={e => setRoom(e.target.value)} />
        </Form.Group>
        <Button variant="success" type="submit" disabled={!user || !room}>Join</Button>
    </Form>
}

export default Lobby;