// frontend/src/CourseUpdates.js

import React, { useEffect } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';

const CourseUpdates = () => {
    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl("/courseHub")
            .build();

        connection.on("ReceiveCourseUpdate", (courseDto) => {
            console.log("New Course Update:", courseDto);
            // Implement your logic to update state/UI here
            // e.g., updating the courses list or triggering a fetch
        });

        connection.start().catch(err => console.error(err.toString()));

        // Cleanup on unmount
        return () => {
            connection.stop();
        };
    }, []);

    return (
        <div>
            <h2>Real-Time Course Updates</h2>
            {/* You can add additional UI elements or state here */}
        </div>
    );
};

export default CourseUpdates;
