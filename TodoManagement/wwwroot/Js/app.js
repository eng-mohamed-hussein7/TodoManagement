`const tasksList = document.getElementById('tasks-list');
const taskCount = document.getElementById('task-count');
const addTaskBtn = document.getElementById('add-task-btn');
const taskModal = document.getElementById('task-modal');
const deleteModal = document.getElementById('delete-modal');
const modalTitle = document.getElementById('modal-title');
const taskForm = document.getElementById('task-form');
const cancelBtn = document.getElementById('cancel-btn');
const closeButtons = document.querySelectorAll('.close');
const searchInput = document.getElementById('search');
const sortBySelect = document.getElementById('sort-by');
const filterButtons = document.querySelectorAll('.filter-btn');
const cancelDeleteBtn = document.getElementById('cancel-delete-btn');
const confirmDeleteBtn = document.getElementById('confirm-delete-btn');`// Task data structure
let tasks = [];
let currentTaskId = null;
let currentFilter = 'all';
let currentPriorityFilter = 'all';
let searchQuery = '';

// Initialize the app
function init() {
loadTasks();
renderTasks();
setupEventListeners();
}

// Load tasks from localStorage
function loadTasks() {
const storedTasks = localStorage.getItem('tasks');
tasks = storedTasks ? JSON.parse(storedTasks) : [];
}

// Save tasks to localStorage
function saveTasks() {
localStorage.setItem('tasks', JSON.stringify(tasks));
}

// Generate a unique ID (GUID)
function generateGuid() {
return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
const r = Math.random() * 16 | 0;
const v = c === 'x' ? r : (r & 0x3 | 0x8);
return v.toString(16);
});
}

// Format date for display
function formatDate(dateString) {
if (!dateString) return 'No due date';
const date = new Date(dateString);
return date.toLocaleString('en-US', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
});
}

// Format relative time
function formatRelativeTime(dateString) {
if (!dateString) return '';
const date = new Date(dateString);
const now = new Date();
const diffMs = date - now;
const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));

if (diffMs < 0) {
    return 'Overdue';
} else if (diffDays === 0) {
    const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
    if (diffHours === 0) {
        const diffMinutes = Math.floor(diffMs / (1000 * 60));
        return `Due in ${diffMinutes} minute${diffMinutes !== 1 ? 's' : ''}`;
    }
    return `Due in ${diffHours} hour${diffHours !== 1 ? 's' : ''}`;
} else if (diffDays === 1) {
    return 'Due tomorrow';
} else {
    return `Due in ${diffDays} days`;
}
}

// Render tasks
function renderTasks() {
// Filter tasks
let filteredTasks = tasks.filter(task => {
// Filter by status
if (currentFilter !== 'all' && task.status !== currentFilter) {
return false;
}
    // Filter by priority
    if (currentPriorityFilter !== 'all' && task.priority !== currentPriorityFilter) {
        return false;
    }
    
    // Filter by search query
    if (searchQuery && !task.title.toLowerCase().includes(searchQuery.toLowerCase()) && 
        (!task.description || !task.description.toLowerCase().includes(searchQuery.toLowerCase()))) {
        return false;
    }
    
    return true;
});

// Sort tasks
const sortBy = sortBySelect.value;
filteredTasks.sort((a, b) => {
    switch (sortBy) {
        case 'dueDate':
            // Sort by due date (tasks without due date go to the end)
            if (!a.dueDate && !b.dueDate) return 0;
            if (!a.dueDate) return 1;
            if (!b.dueDate) return -1;
            return new Date(a.dueDate) - new Date(b.dueDate);
        case 'priority':
            // Sort by priority (high > medium > low)
            const priorityOrder = { high: 0, medium: 1, low: 2 };
            return priorityOrder[a.priority] - priorityOrder[b.priority];
        case 'title':
            // Sort alphabetically by title
            return a.title.localeCompare(b.title);
        case 'createdDate':
            // Sort by created date (newest first)
            return new Date(b.createdDate) - new Date(a.createdDate);
        default:
            return 0;
    }
});

// Update task count
taskCount.textContent = `(${filteredTasks.length})`;

// Clear tasks list
tasksList.innerHTML = '';

// Show empty state if no tasks
if (filteredTasks.length === 0) {
    tasksList.innerHTML = `
        <div class="empty-state">
            <i class="fas fa-clipboard-list"></i>
            <h3>No tasks found</h3>
            <p>Create a new task or adjust your filters to see tasks.</p>
            <button id="empty-add-task" class="primary-btn">
                <i class="fas fa-plus"></i> Add New Task
            </button>
        </div>
    `;
    
    document.getElementById('empty-add-task').addEventListener('click', openAddTaskModal);
    return;
}

// Render each task
filteredTasks.forEach(task => {
    const taskElement = document.createElement('div');
    taskElement.className = 'task-card';
    taskElement.dataset.id = task.id;
    
    // Determine if task is overdue
    let isOverdue = false;
    if (task.dueDate && task.status !== 'completed') {
        isOverdue = new Date(task.dueDate) &lt; new Date();
    }
    
    taskElement.innerHTML = `
        <div class="task-header">
            <h3 class="task-title">${task.title}</h3>
            <div class="task-actions">
                <button class="task-action-btn edit" title="Edit Task">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="task-action-btn delete" title="Delete Task">
                    <i class="fas fa-trash"></i>
                </button>
            </div>
        </div>
        ${task.description ? `<p class="task-description">${task.description}</p>` : ''}
        <div class="task-meta">
            <span class="task-status ${task.status}">${task.status.charAt(0).toUpperCase() + task.status.slice(1)}</span>
            <span class="task-priority ${task.priority}">${task.priority.charAt(0).toUpperCase() + task.priority.slice(1)}</span>
            ${task.dueDate ? `
                <span class="task-meta-item ${isOverdue ? 'overdue' : ''}">
                    <i class="fas fa-calendar-alt"></i>
                    ${formatRelativeTime(task.dueDate)}
                </span>
            ` : ''}
            <span class="task-meta-item">
                <i class="fas fa-clock"></i>
                Created ${formatDate(task.createdDate)}
            </span>
        </div>
    `;
    
    // Add event listeners to task actions
    taskElement.querySelector('.edit').addEventListener('click', () => openEditTaskModal(task.id));
    taskElement.querySelector('.delete').addEventListener('click', () => openDeleteModal(task.id));
    
    tasksList.appendChild(taskElement);
});
}

// Setup event listeners
function setupEventListeners() {
// Add task button
addTaskBtn.addEventListener('click', openAddTaskModal);
// Close modal buttons
closeButtons.forEach(button => {
    button.addEventListener('click', () => {
        taskModal.style.display = 'none';
        deleteModal.style.display = 'none';
    });
});

// Cancel button
cancelBtn.addEventListener('click', () => {
    taskModal.style.display = 'none';
});

// Cancel delete button
cancelDeleteBtn.addEventListener('click', () => {
    deleteModal.style.display = 'none';
});

// Confirm delete button
confirmDeleteBtn.addEventListener('click', deleteTask);

// Task form submit
taskForm.addEventListener('submit', saveTask);

// Search input
searchInput.addEventListener('input', () => {
    searchQuery = searchInput.value.trim();
    renderTasks();
});

// Sort select
sortBySelect.addEventListener('change', renderTasks);

// Filter buttons
filterButtons.forEach(button => {
    button.addEventListener('click', () => {
        // Determine which type of filter was clicked
        if (button.dataset.filter) {
            // Status filter
            document.querySelectorAll('.filter-btn[data-filter]').forEach(btn => {
                btn.classList.remove('active');
            });
            button.classList.add('active');
            currentFilter = button.dataset.filter;
        } else if (button.dataset.priority) {
            // Priority filter
            document.querySelectorAll('.filter-btn[data-priority]').forEach(btn => {
                btn.classList.remove('active');
            });
            button.classList.add('active');
            currentPriorityFilter = button.dataset.priority;
        }
        
        renderTasks();
    });
});

// Close modals when clicking outside
window.addEventListener('click', (event) => {
    if (event.target === taskModal) {
        taskModal.style.display = 'none';
    }
    if (event.target === deleteModal) {
        deleteModal.style.display = 'none';
    }
});
}

// Open add task modal
function openAddTaskModal() {
modalTitle.textContent = 'Add New Task';
taskForm.reset();
document.getElementById('task-id').value = '';
// Set default values
document.getElementById('status').value = 'pending';
document.getElementById('priority').value = 'medium';

// Set min date for due date to today
const today = new Date();
today.setMinutes(today.getMinutes() - today.getTimezoneOffset());

taskModal.style.display = 'block';
}

// Open edit task modal
function openEditTaskModal(taskId) {
const task = tasks.find(t => t.id === taskId);
if (!task) return;
modalTitle.textContent = 'Edit Task';
document.getElementById('task-id').value = task.id;
document.getElementById('title').value = task.title;
document.getElementById('description').value = task.description || '';
document.getElementById('status').value = task.status;
document.getElementById('priority').value = task.priority;

if (task.dueDate) {
    // Format date for datetime-local input
    const dueDate = new Date(task.dueDate);
    dueDate.setMinutes(dueDate.getMinutes() - dueDate.getTimezoneOffset());
    document.getElementById('dueDate').value = dueDate.toISOString().slice(0, 16);
} else {
    document.getElementById('dueDate').value = '';
}

taskModal.style.display = 'block';
}

// Open delete modal
function openDeleteModal(taskId) {
currentTaskId = taskId;
deleteModal.style.display = 'block';
}

// Save task
function saveTask(event) {
event.preventDefault();
const taskId = document.getElementById('task-id').value;
const title = document.getElementById('title').value.trim();
const description = document.getElementById('description').value.trim();
const status = document.getElementById('status').value;
const priority = document.getElementById('priority').value;
const dueDate = document.getElementById('dueDate').value;

// Validate title (required, max 100 chars)
if (!title) {
    alert('Title is required');
    return;
}

if (title.length > 100) {
    alert('Title must be less than 100 characters');
    return;
}

const now = new Date();

if (taskId) {
    // Edit existing task
    const taskIndex = tasks.findIndex(t => t.id === taskId);
    if (taskIndex !== -1) {
        tasks[taskIndex] = {
            ...tasks[taskIndex],
            title,
            description,
            status,
            priority,
            dueDate: dueDate || null,
            lastModifiedDate: now.toISOString()
        };
    }
} else {
    // Add new task
    const newTask = {
        id: generateGuid(),
        title,
        description,
        status,
        priority,
        dueDate: dueDate || null,
        createdDate: now.toISOString(),
        lastModifiedDate: now.toISOString()
    };
    
    tasks.push(newTask);
}

saveTasks();
renderTasks();
taskModal.style.display = 'none';
}

// Delete task
function deleteTask() {
if (!currentTaskId) return;
tasks = tasks.filter(task => task.id !== currentTaskId);
saveTasks();
renderTasks();
deleteModal.style.display = 'none';
currentTaskId = null;
tasks = tasks.filter(task => task.id !== currentTaskId);
saveTasks();
renderTasks();
deleteModal.style.display = 'none';
currentTaskId = null;
}

// Initialize the app
document.addEventListener('DOMContentLoaded', init);