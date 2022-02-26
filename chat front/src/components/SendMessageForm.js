import { Form, Button, FormControl, InputGroup } from 'react-bootstrap';
import { useState } from 'react';

const SendMessageForm = ({ sendMessage, fromuser, rooom }) => {
    const [message, setMessage] = useState('');

    return <Form
        onSubmit={e => {
            e.preventDefault();  
            sendMessage(message);
            const m = {
                "message": message,
                "fromuser": fromuser,
                "rooom": rooom
            }
            fetch('https://localhost:44382/api/Messages/PostMessage/', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(m)
            })
                .then(() => {
                    console.log(m);
                });

            setMessage('');
        }}>
        <InputGroup>
            <FormControl type="user" placeholder="message..."
                onChange={e => setMessage(e.target.value)} value={message} />
            <InputGroup.Append>
                <Button variant="primary" type="submit" disabled={!message}>Send</Button>
            </InputGroup.Append>
        </InputGroup>
    </Form>
}

export default SendMessageForm;