document.addEventListener('DOMContentLoaded', () => {
    // WebSocket connection setup
    let notificationSocket = null;

    function connectWebSocket() {
        const token = localStorage.getItem('authToken');
        if (!token) {
            console.error('[CUSTOMER] No auth token found, delaying WebSocket connection');
            
            return;
        }

        console.log('[CUSTOMER] Auth token:', token);
        notificationSocket = new WebSocket(`wss://localhost:44351/ws/notifications?token=${token}`);

notificationSocket.onopen = () => {
    console.log('[CUSTOMER] WebSocket connection established.');
};

notificationSocket.onmessage = (event) => {
    try {
        const notification = JSON.parse(event.data);
        console.log('[CUSTOMER] Received message:', notification);
                displayNotification(notification);
    } catch (error) {
        console.error('[CUSTOMER] Error parsing notification:', error);
    }
};

notificationSocket.onclose = (event) => {
    console.log('[CUSTOMER] WebSocket connection closed:', event);
            setTimeout(connectWebSocket, 3000); // Reconnect after 5 seconds
};

notificationSocket.onerror = (error) => {
    console.error('[CUSTOMER] WebSocket error:', error);
};
    }

    // Function to display notifications
    function displayNotification(notification) {
    const notificationArea = document.getElementById('notification-area');
        if (!notificationArea) {
            console.warn('[CUSTOMER] Notification area not found on this page');
            return;
        }

        let message = '';
        switch (notification.type) {
            case 'order_confirmed':
                message = `<strong>Order Confirmed!</strong><br>Order ID: ${notification.orderId}<br>Order Date: ${new Date(notification.orderDate).toLocaleString()}<br>Claim Code: ${notification.claimCode}<br>Total Price: Rs ${notification.totalPrice.toFixed(2)}`;
                break;
            case 'order_completed':
                message = `<strong>Order Completed!</strong><br>Order ID: ${notification.orderId}<br>Claim Code: ${notification.claimCode}<br>Total Price: ${notification.totalPrice.toFixed(2)}`;
                break;
            
            default:
                return; // Ignore non-order-related notifications
        }

        const notificationElement = document.createElement('div');
        notificationElement.classList.add('notification', 'alert', 'alert-success', 'alert-dismissible', 'fade', 'show');
        notificationElement.setAttribute('role', 'alert');
        notificationElement.innerHTML = `
            ${message}
            <button type="button" class="btn close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true">&times;</span>
            </button>
        `;
        notificationArea.prepend(notificationElement);
        console.log('[CUSTOMER] Displayed notification for type:', notification.type, 'Order ID:', notification.orderId);

        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            $(notificationElement).alert('close');
        }, 3000);
    }

    // Start WebSocket connection
    connectWebSocket();

    // Inject CSS for notifications
    const style = document.createElement('style');
    style.textContent = `
        #notification-area {
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 1050;
            width: 300px;
        }
        .notification {
            margin-bottom: 10px;
            padding: 15px;
            border-radius: 5px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
            font-size: 0.9rem;
    }
        .notification .close {
            padding: 0.5rem;
            opacity: 0.7;
}
    `;
    document.head.appendChild(style);
});

// Ensure jQuery is available for alert dismissal
if (typeof jQuery === 'undefined') {
    console.error('[CUSTOMER] jQuery is required for notification dismissal');
}