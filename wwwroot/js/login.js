document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    const errorMessageDiv = document.getElementById('errorMessage');

    loginForm.addEventListener('submit', async (event) => {
        event.preventDefault();

        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;

        const userData = {
            email: email,
            password: password
        };

        try {
            const response = await fetch('/api/Auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(userData)
            });

            const data = await response.json();

            if (response.ok) {
                Toastify({
                    text: data.message || "Login successful!",
                    duration: 3000,
                    close: true,
                    gravity: "top",
                    position: "right",
                    style: {
                        background: "linear-gradient(to right, #00b09b, #96c93d)",
                    }
                }).showToast();

                const token = data.token;
                const role = data.role; // Get the role from the response
                console.log('Login successful, Token:', token, 'Role:', role);
                localStorage.setItem('authToken', token);

                let redirectUrl = 'dashboard.html'; // Default redirect

                if (role === 'Admin') {
                    redirectUrl = 'adminDashboard.html'; // Replace with your admin dashboard URL
                } else if (role === 'Staff') {
                    redirectUrl = 'staffHome.html'; // Replace with your staff dashboard URL
                } else if (role === 'User') {
                    redirectUrl = 'userHome.html'; // Replace with your customer dashboard URL
                }

                setTimeout(() => {
                    window.location.href = redirectUrl;
                }, 3500);

            } else if (response.status === 401) {
                const errorData = await response.json(); // Expect JSON error for unauthorized
                Toastify({
                    text: errorData.message || "Invalid credentials.",
                    duration: 5000,
                    close: true,
                    gravity: "top",
                    position: "right",
                    style: {
                        background: "#dc3545",
                    }
                }).showToast();
                console.error('Login failed:', errorData);
            } else {
                const errorText = await response.text();
                Toastify({
                    text: `Login failed with status: ${response.status} - ${errorText}`,
                    duration: 5000,
                    close: true,
                    gravity: "top",
                    position: "right",
                    style: {
                        background: "#dc3545",
                    }
                }).showToast();
                console.error('Login failed with status:', response.status, errorText);
            }
        } catch (error) {
            Toastify({
                text: 'An unexpected error occurred during login.',
                duration: 5000,
                close: true,
                gravity: "top",
                position: "right",
                style: {
                    background: "#dc3545",
                }
            }).showToast();
            console.error('Fetch error:', error);
        }
    });
});