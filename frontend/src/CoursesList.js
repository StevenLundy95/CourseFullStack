import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { HubConnectionBuilder } from '@microsoft/signalr'; // Import the HubConnectionBuilder
import {
    Container,
    Typography,
    Button,
    TextField,
    Card,
    CardContent,
    Grid,
    Snackbar,
    Alert
} from '@mui/material';

const CoursesList = () => {
    const [courses, setCourses] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [newCourse, setNewCourse] = useState({ name: '', description: '', noOfChapters: '', instructorId: '' });
    const [snackbarOpen, setSnackbarOpen] = useState(false);

    const fetchCourses = async () => {
        try {
            const response = await axios.get("http://localhost:5003/courses");
            setCourses(response.data);
            setLoading(false);
        } catch (err) {
            setError('Error fetching data');
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchCourses();

        // Establish SignalR connection
        const connection = new HubConnectionBuilder()
            .withUrl("http://localhost:5003/courseHub") // Make sure this URL matches your hub route
            .build();

        // Start the connection
        connection.start()
            .then(() => console.log('SignalR Connected'))
            .catch(err => console.error('Error while starting SignalR connection: ', err));

        // Listen for updates
        connection.on("ReceiveCourseUpdate", (course) => {
            setCourses(prevCourses => [...prevCourses, course]); // Update state with new course
        });

        // Cleanup connection on component unmount
        return () => {
            connection.stop();
        };
    }, []);

    const addCourse = async () => {
        try {
            await axios.post("http://localhost:5003/courses", newCourse);
            setNewCourse({ name: '', description: '', noOfChapters: '', instructorId: '' });
            setSnackbarOpen(true);
        } catch (err) {
            setError('Error adding course');
        }
    };

    const deleteAllCourses = async () => {
        try {
            await axios.delete("http://localhost:5003/courses");
            setCourses([]);
        } catch (err) {
            setError('Error deleting courses');
        }
    };

    if (loading) return <div>Loading...</div>;
    if (error) return <Alert severity="error">{error}</Alert>;

    return (
        <Container>
            <Typography variant="h2" align="center" gutterBottom>
                Course List
            </Typography>
            <Button variant="contained" color="error" onClick={deleteAllCourses} style={{ marginBottom: '20px' }}>
                Delete All Courses
            </Button>
            <Grid container spacing={3}>
                {courses.map(course => (
                    <Grid item xs={12} sm={6} md={4} key={course.id}>
                        <Card>
                            <CardContent>
                                <Typography variant="h5">{course.name}</Typography>
                                <Typography variant="body2">{course.description}</Typography>
                                <Typography variant="body1">Chapters: {course.noOfChapters}</Typography>
                                <Typography variant="body1">Instructor ID: {course.instructorId}</Typography>
                            </CardContent>
                        </Card>
                    </Grid>
                ))}
            </Grid>
            <Typography variant="h4" align="center" gutterBottom style={{ marginTop: '40px' }}>
                Add a New Course
            </Typography>
            <Grid container spacing={2} justifyContent="center">
                <Grid item xs={12} sm={6} md={3}>
                    <TextField
                        fullWidth
                        variant="outlined"
                        label="Name"
                        value={newCourse.name}
                        onChange={e => setNewCourse({ ...newCourse, name: e.target.value })}
                    />
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                    <TextField
                        fullWidth
                        variant="outlined"
                        label="Description"
                        value={newCourse.description}
                        onChange={e => setNewCourse({ ...newCourse, description: e.target.value })}
                    />
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                    <TextField
                        fullWidth
                        variant="outlined"
                        label="No of Chapters"
                        type="number"
                        value={newCourse.noOfChapters}
                        onChange={e => setNewCourse({ ...newCourse, noOfChapters: e.target.value })}
                    />
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                    <TextField
                        fullWidth
                        variant="outlined"
                        label="Instructor ID"
                        value={newCourse.instructorId}
                        onChange={e => setNewCourse({ ...newCourse, instructorId: e.target.value })}
                    />
                </Grid>
            </Grid>
            <Button variant="contained" color="primary" onClick={addCourse} style={{ marginTop: '20px' }}>
                Add Course
            </Button>
            <Snackbar open={snackbarOpen} autoHideDuration={6000} onClose={() => setSnackbarOpen(false)}>
                <Alert onClose={() => setSnackbarOpen(false)} severity="success">
                    Course added successfully!
                </Alert>
            </Snackbar>
        </Container>
    );
};

export default CoursesList;
