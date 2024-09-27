// frontend/src/App.js

import React from 'react';
import './App.css';
import CoursesList from './CoursesList'; // Import the CoursesList component
import CourseUpdates from './CourseUpdates'; // Import the CourseUpdates component

function App() {
    return (
        <div className="App">
            <header className="App-header">
                <h1>Course Management</h1>
                {/* Render the CoursesList component to display the courses */}
                <CoursesList />
                {/* Render the CourseUpdates component for real-time updates */}
                <CourseUpdates />
            </header>
        </div>
    );
}

export default App;
