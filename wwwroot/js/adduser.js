
const deleteSVG = `
<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24">
<path
                                fill="currentColor"
                                d="M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6zM19 4h-3.5l-1-1h-5l-1 1H5v2h14z"
                                stroke-width="0"
                                stroke="currentColor"
                              />
                            </svg>`;

const editSVG = `
<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24">
  <path fill="currentColor" d="M15.748 2.947a2 2 0 0 1 2.828 0l2.475 2.475a2 2 0 0 1 0 2.829L9.158 20.144l-6.38 1.076l1.077-6.38zm-.229 3.057l2.475 2.475l1.643-1.643l-2.475-2.474zm1.06 3.89l-2.474-2.475l-8.384 8.384l-.503 2.977l2.977-.502z"/>
</svg>`;

const saveSVG = `
<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24">
  <path fill="currentColor" d="M20.285 6.709l-11.2 11.2l-5.657-5.657L5.8 10.88l3.286 3.285L18.17 5.08z"/>
</svg>`;
let currentlyEditingRow = null;
const staffTableBody = document.querySelector(".table tbody");
document.addEventListener("DOMContentLoaded", function () {
   

    // Function to fetch staff users from the API
    
    fetchStaffUsers();
});
function fetchStaffUsers() {
    const token = localStorage.getItem('authToken');
    fetch('/api/User/add', {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    })
        .then(response => {
            console.log('[STAFF] API response status:', response.status);
            if (!response.ok) {
                if (response.status === 401) {
                    localStorage.removeItem('authToken');
                    window.location.href = 'login.html';
                }
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(staffUsersResponse => {
            console.log('[STAFF] API response data:', staffUsersResponse);
            loadingSpinner.style.display = "none";
            errorMessage.style.display = "none";

            // Handle response as a direct array or $values
            const staffUsersArray = Array.isArray(staffUsersResponse)
                ? staffUsersResponse
                : (staffUsersResponse.$values && Array.isArray(staffUsersResponse.$values)
                    ? staffUsersResponse.$values
                    : []);
            console.log('[STAFF] Processed users array:', staffUsersArray);

            // Clear the existing table rows
            staffTableBody.innerHTML = '';

            if (staffUsersArray.length === 0) {
                staffTableBody.innerHTML = `
                    <tr>
                        <td colspan="7" class="text-center">No staff users found.</td>
                    </tr>
                `;
                return;
            }

            // Populate the table with the fetched data
            staffUsersArray.forEach(user => {
                const row = staffTableBody.insertRow();
                row.insertCell().textContent = user.id;
                row.insertCell().textContent = `${user.firstName} ${user.lastName}`;
                row.insertCell().textContent = user.email;
                row.insertCell().textContent = user.address || '';
                row.insertCell().textContent = user.phoneNumber || '';
                row.insertCell().textContent = user.role;
                const actionsCell = row.insertCell();
                actionsCell.classList.add("actionBtns", "d-flex", "justify-content-between");

                const editButton = document.createElement('button');
                editButton.classList.add('edit-Btn');
                editButton.innerHTML = editSVG;
                editButton.addEventListener('click', function () {
                    handleEdit(this.closest('tr'), user);
                });
                actionsCell.appendChild(editButton);

                const deleteButton = document.createElement('button');
                deleteButton.innerHTML = deleteSVG;
                deleteButton.addEventListener('click', function () {
                    handleDelete(user.id);
                });
                actionsCell.appendChild(deleteButton);
            });
        })
        .catch(error => {
            console.error('[STAFF] Error fetching staff users:', error);
            loadingSpinner.style.display = "none";
            errorMessage.style.display = "block";
            errorMessage.textContent = `Failed to load staff users: ${error.message}`;
        });
}
function toggleEditMode(row, enable) {
    const editBtn = row.querySelector(".edit-Btn");
    row.classList.toggle("editing", enable);

    Array.from(row.children).forEach((cell, index) => {
        if (index < 6) {
            if (enable) {
                const value = cell.innerText;
                cell.setAttribute("data-original", value);
                cell.innerHTML = `<input type="text" class="form-control" value="${value}" />`;
            } else {
                const input = cell.querySelector("input");
                if (input) {
                    cell.textContent = input.value;
                }
            }
        }
    });

    editBtn.innerHTML = enable ? saveSVG : editSVG;
}

function handleEdit(row, userData) {
    console.log('Edit clicked for user:', userData);
    const isEditing = row.classList.contains("editing");

    if (!isEditing) {
        if (currentlyEditingRow && currentlyEditingRow !== row) {
            toggleEditMode(currentlyEditingRow, false);
        }
        toggleEditMode(row, true);
        currentlyEditingRow = row;
    } else {
        // Save the edited data
        const updatedData = {
            id: userData.id, // Use the ID from the userData object
            firstName: '',
            lastName: '',
            email: '',
            address: '',
            phoneNumber: '',
            role: ''
        };
        Array.from(row.children).forEach((cell, index) => {
            if (index === 1) {
                const nameParts = cell.querySelector('input').value.split(' ');
                updatedData.firstName = nameParts[0];
                updatedData.lastName = nameParts.slice(1).join(' ');
            }
            if (index === 2) updatedData.email = cell.querySelector('input').value;
            if (index === 3) updatedData.address = cell.querySelector('input').value;
            if (index === 4) updatedData.phoneNumber = cell.querySelector('input').value;
            if (index === 5) updatedData.role = cell.querySelector('input').value;
        });
        console.log('Saving updated data:', updatedData);
        // Make an API call to update the user data on the backend
        fetch(`/api/User/update/${updatedData.id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(updatedData),
        })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(err => {
                        throw new Error(err.message || 'Failed to update user');
                    });
                }
                return response.json();
            })
            .then(data => {
                console.log('User updated successfully:', data);
                toggleEditMode(row, false); // Exit edit mode
                fetchStaffUsers(); // Refresh the table
            })
            .catch(error => {
                console.error('Error updating user:', error);
                alert(error.message);
                alert(error.message);
            });

        toggleEditMode(row, false);
        currentlyEditingRow = null;
    }
}

function handleDelete(userId) {
    if (confirm(`Are you sure you want to delete user with ID: ${userId}?`)) {
        // Implement your delete API call here
        console.log('Deleting user with ID:', userId);
        fetch(`/api/User/delete/${userId}`, { method: 'DELETE' })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(err => {
                        throw new Error(err.message || 'Failed to delete user');
                    });
                }
                console.log('User deleted successfully');
                fetchStaffUsers(); // Refresh the table
            })
            .catch(error => {
                console.error('Error deleting user:', error);
                alert(error.message);
            });
    }
}
    //add user submission
    document.getElementById("staffForm").addEventListener("submit", function (e) {
        e.preventDefault();

        const firstName = document.getElementById("firstName").value;
        const lastName = document.getElementById("lastName").value;
        const email = document.getElementById("email").value;
        const role = document.getElementById("role").value;
        const phone = document.getElementById("phone").value;
        const address = document.getElementById("address").value;
        const password = document.getElementById("password").value;
        const confirmPassword = document.getElementById("confirmPassword").value;

        if (password !== confirmPassword) {
            alert("Passwords do not match!");
            return;
        }

        const userData = {
            firstName: firstName,
            lastName: lastName,
            email: email,
            password: password,
            confirmPassword: confirmPassword,
            address: address,
            role: role,
        };
        //Authorization
        const adminToken = localStorage.getItem('authToken');

        const headers = {
            'Content-Type': 'application/json',
        };

        // Add the Authorization header with the Bearer token if it exists
        if (adminToken) {
            headers['Authorization'] = `Bearer ${adminToken}`;
        }
        //Authorization End
        fetch('/api/Auth/register', {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(userData)
        })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(err => {
                        throw new Error(err.message || 'Registration failed');
                    });
                }
                return response.json();
            })
            .then(data => {
                console.log('Registration successful:', data);
                $('#staffModal').modal('hide');
                this.reset();
                
            })
            .catch(error => {
                console.error('Error during registration:', error);
                alert(error.message);
            });
    });

