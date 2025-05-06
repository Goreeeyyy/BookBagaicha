document.addEventListener('DOMContentLoaded', () => {
    const registerForm = document.getElementById('registerForm');
    const errorMessageDiv = document.getElementById('errorMessage');
    const successMessageDiv = document.getElementById('successMessage');

    registerForm.addEventListener('submit', async (event) => {
        event.preventDefault();

        const firstName = document.getElementById('firstName').value;
        const lastName = document.getElementById('lastName').value;
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirmPassword').value;
        

        if (password !== confirmPassword) {
            errorMessageDiv.textContent = "Passwords do not match.";
            successMessageDiv.textContent = "";
            return;
        }

        const userData = {
            firstName: firstName,
            lastName: lastName,
            email: email,
            password: password,
            confirmPassword: confirmPassword,
            address: "Default Address", // You might want to add an address field in the form
            role:"User"        };

        try {
            const response = await fetch('https://localhost:44351/api/Auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(userData)
            });

            const data = await response.json();

            if (response.ok) {
                Toastify({
                    text: data.message || "Registration successful!",
                    duration: 3000,
                    close: true,
                    gravity: "top", // `top` or `bottom`
                    position: "right", // `left`, `center` or `right`
                    style: {
                        background: "linear-gradient(to right, #00b09b, #96c93d)",
                    }
                }).showToast();

                // Redirect to login page after a delay
                setTimeout(() => {
                    window.location.href = 'pages/login.html';
                }, 3500); 
            } else {
                errorMessageDiv.textContent = data.error || data.errors ? (data.errors[0]?.description || "Registration failed.") : "Registration failed.";
                successMessageDiv.textContent = "";
                console.error('Registration failed:', data);
            }
        } catch (error) {
            errorMessageDiv.textContent = 'An unexpected error occurred.';
            successMessageDiv.textContent = "";
            console.error('Fetch error:', error);
        }
    });
});
