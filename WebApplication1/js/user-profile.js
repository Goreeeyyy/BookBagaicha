document.addEventListener('DOMContentLoaded', () => {
    // Load user data from localStorage
    let user = JSON.parse(localStorage.getItem('user')) || {
        fullName: 'Alexa Rawles',
        email: 'olesorawles@gmail.com',
        gender: 'Female',
        language: 'English',
        nickName: 'Alex',
        country: 'United States',
        timeZone: '(UTC-05:00) Eastern Time'
    };

    // Initialize form
    initializeForm(user);

    // Edit Profile Button
    const editBtn = document.getElementById('editBtn');
    const saveBtn = document.getElementById('saveBtn');
    const cancelBtn = document.getElementById('cancelBtn');

    editBtn.addEventListener('click', enableEditing);
    saveBtn.addEventListener('click', saveChanges);
    cancelBtn.addEventListener('click', cancelEditing);

    // Logout Button
    document.getElementById('logoutBtn').addEventListener('click', logout);

    // Newsletter Subscription
    document.getElementById('newsletterBtn').addEventListener('click', subscribeNewsletter);

    function initializeForm(userData) {
        document.getElementById('fullName').value = userData.fullName;
        document.getElementById('userName').textContent = userData.fullName;
        document.getElementById('userEmail').textContent = userData.email;
        document.getElementById('gender').value = userData.gender;
        document.getElementById('language').value = userData.language;
        document.getElementById('nickName').value = userData.nickName;
        document.getElementById('country').value = userData.country;
        document.getElementById('timeZone').value = userData.timeZone;
    }

    function enableEditing() {
        const inputs = document.querySelectorAll('#profileForm input, #profileForm select');
        inputs.forEach(input => {
            input.readOnly = false;
            input.disabled = false;
        });

        editBtn.classList.add('d-none');
        saveBtn.classList.remove('d-none');
        cancelBtn.classList.remove('d-none');
    }

    function saveChanges() {
        const updatedUser = {
            fullName: document.getElementById('fullName').value,
            email: document.getElementById('userEmail').textContent,
            gender: document.getElementById('gender').value,
            language: document.getElementById('language').value,
            nickName: document.getElementById('nickName').value,
            country: document.getElementById('country').value,
            timeZone: document.getElementById('timeZone').value
        };

        localStorage.setItem('user', JSON.stringify(updatedUser));
        initializeForm(updatedUser);
        cancelEditing();
        alert('Profile updated successfully!');
    }

    function cancelEditing() {
        const inputs = document.querySelectorAll('#profileForm input, #profileForm select');
        inputs.forEach(input => {
            input.readOnly = true;
            input.disabled = true;
        });

        editBtn.classList.remove('d-none');
        saveBtn.classList.add('d-none');
        cancelBtn.classList.add('d-none');
        initializeForm(JSON.parse(localStorage.getItem('user')));
    }

    function logout() {
        localStorage.removeItem('authToken');
        window.location.href = 'index.html';
    }

    function subscribeNewsletter() {
        const email = document.getElementById('newsletterEmail').value;
        if (validateEmail(email)) {
            localStorage.setItem('subscribedEmail', email);
            alert('Thank you for subscribing!');
        } else {
            alert('Please enter a valid email address');
        }
    }

    function validateEmail(email) {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(String(email).toLowerCase());
    }
});