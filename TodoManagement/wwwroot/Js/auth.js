`// DOM Elements
const loginForm = document.getElementById('login-form');
const registerForm = document.getElementById('register-form');`// Initialize auth
function initAuth() {
setupAuthEventListeners();
}

// Setup event listeners
function setupAuthEventListeners() {
// Login form
if (loginForm) {
loginForm.addEventListener('submit', handleLogin);
}
// Register form
if (registerForm) {
    registerForm.addEventListener('submit', handleRegister);
}
}

// Handle login
function handleLogin(event) {
event.preventDefault();
const email = document.getElementById('email').value.trim();
const password = document.getElementById('password').value;
const remember = document.getElementById('remember').checked;

// In a real app, you would validate and send this to a server
// For this demo, we'll just simulate a successful login

// Simple validation
if (!email || !password) {
    showMessage('Please enter both email and password', 'error');
    return;
}

// Simulate API call
simulateApiCall(() => {
    // Store user info in localStorage
    const user = {
        email,
        name: email.split('@')[0], // Just for demo purposes
        isLoggedIn: true
    };
    
    localStorage.setItem('currentUser', JSON.stringify(user));
    
    // Redirect to dashboard
    showMessage('Login successful! Redirecting...', 'success');
    setTimeout(() => {
        window.location.href = 'index.html';
    }, 1500);
});
}

// Handle register
function handleRegister(event) {
event.preventDefault();
const name = document.getElementById('name').value.trim();
const email = document.getElementById('reg-email').value.trim();
const password = document.getElementById('reg-password').value;
const confirmPassword = document.getElementById('confirm-password').value;
const termsAccepted = document.getElementById('terms').checked;

// Simple validation
if (!name || !email || !password || !confirmPassword) {
    showMessage('Please fill in all fields', 'error');
    return;
}

if (password.length < 8) {
    showMessage('Password must be at least 8 characters long', 'error');
    return;
}

if (password !== confirmPassword) {
    showMessage('Passwords do not match', 'error');
    return;
}

if (!termsAccepted) {
    showMessage('You must accept the Terms of Service', 'error');
    return;
}

// Simulate API call
simulateApiCall(() => {
    // Store user info in localStorage
    const user = {
        name,
        email,
        isLoggedIn: true
    };
    
    localStorage.setItem('currentUser', JSON.stringify(user));
    
    // Redirect to dashboard
    showMessage('Account created successfully! Redirecting...', 'success');
    setTimeout(() => {
        window.location.href = 'index.html';
    }, 1500);
});
}

// Show message
function showMessage(message, type = 'info') {
// Check if message container exists
let messageContainer = document.querySelector('.message-container');
// If not, create one
if (!messageContainer) {
    messageContainer = document.createElement('div');
    messageContainer.className = 'message-container';
    document.body.appendChild(messageContainer);
    
    // Style the message container
    messageContainer.style.position = 'fixed';
    messageContainer.style.top = '20px';
    messageContainer.style.left = '50%';
    messageContainer.style.transform = 'translateX(-50%)';
    messageContainer.style.zIndex = '1000';
    messageContainer.style.width = '300px';
}

// Create message element
const messageElement = document.createElement('div');
messageElement.className = `message ${type}`;
messageElement.textContent = message;

// Style the message
messageElement.style.padding = '10px 15px';
messageElement.style.marginBottom = '10px';
messageElement.style.borderRadius = '4px';
messageElement.style.boxShadow = '0 2px 4px rgba(0,0,0,0.1)';
messageElement.style.animation = 'fadeIn 0.3s ease-out';

// Set color based on type
if (type === 'error') {
    messageElement.style.backgroundColor = '#fee2e2';
    messageElement.style.color = '#dc2626';
    messageElement.style.borderLeft = '4px solid #dc2626';
} else if (type === 'success') {
    messageElement.style.backgroundColor = '#d1fae5';
    messageElement.style.color = '#059669';
    messageElement.style.borderLeft = '4px solid #059669';
} else {
    messageElement.style.backgroundColor = '#e0f2fe';
    messageElement.style.color = '#0284c7';
    messageElement.style.borderLeft = '4px solid #0284c7';
}

// Add message to container
messageContainer.appendChild(messageElement);

// Remove message after 5 seconds
setTimeout(() => {
    messageElement.style.animation = 'fadeOut 0.3s ease-out';
    setTimeout(() => {
        messageContainer.removeChild(messageElement);
        
        // Remove container if empty
        if (messageContainer.children.length === 0) {
            document.body.removeChild(messageContainer);
        }
    }, 300);
}, 5000);
}

// Simulate API call with loading state
function simulateApiCall(callback) {
// Create loading spinner
const loadingSpinner = document.createElement('div');
loadingSpinner.className = 'loading-spinner';
loadingSpinner.innerHTML = '`<i class="fas fa-circle-notch fa-spin">``</i>`';
// Style the spinner
loadingSpinner.style.position = 'fixed';
loadingSpinner.style.top = '0';
loadingSpinner.style.left = '0';
loadingSpinner.style.width = '100%';
loadingSpinner.style.height = '100%';
loadingSpinner.style.backgroundColor = 'rgba(255, 255, 255, 0.7)';
loadingSpinner.style.display = 'flex';
loadingSpinner.style.justifyContent = 'center';
loadingSpinner.style.alignItems = 'center';
loadingSpinner.style.zIndex = '1000';
loadingSpinner.style.fontSize = '2rem';
loadingSpinner.style.color = '#4f46e5';

// Add spinner to body
document.body.appendChild(loadingSpinner);

// Simulate API delay
setTimeout(() => {
    // Remove spinner
    document.body.removeChild(loadingSpinner);
    
    // Execute callback
    callback();
}, 1500);
}

// Check if user is logged in
function checkAuth() {
const currentUser = JSON.parse(localStorage.getItem('currentUser'));
if (currentUser && currentUser.isLoggedIn) {
    // Update UI for logged in user
    const userInfo = document.querySelector('.user-info p');
    if (userInfo) {
        userInfo.textContent = currentUser.name;
    }
    
    // Update nav links
    const navLinks = document.querySelector('.nav-links');
    if (navLinks) {
        navLinks.innerHTML = `
            <a href="index.html" class="${window.location.pathname.includes('index.html') ? 'active' : ''}">Home</a>
            <a href="#" id="logout-btn">Logout</a>
        `;
        
        // Add logout event listener
        document.getElementById('logout-btn').addEventListener('click', (e) => {
            e.preventDefault();
            localStorage.removeItem('currentUser');
            window.location.href = 'login.html';
        });
    }
}
}

// Initialize auth and check login status
document.addEventListener('DOMContentLoaded', () => {
initAuth();
checkAuth();
});