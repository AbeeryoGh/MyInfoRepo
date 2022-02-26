import SendMessageForm from './SendMessageForm';
import MessageContainer from './MessageContainer';
import ConnectedUsers from './ConnectedUsers';
import { Button } from 'react-bootstrap';

const Chat = ({ sendMessage,fromuser , rooom, messages, users, closeConnection }) => <div>
    <div className='leave-room'>
        <Button variant='danger' onClick={() => closeConnection()}>Leave Room</Button>
    </div>
    <ConnectedUsers users={users} />
    <div className='chat'>
        <MessageContainer messages={messages} />
        <SendMessageForm sendMessage={sendMessage} rooom={rooom} fromuser={fromuser} />
    </div>
</div>

export default Chat;