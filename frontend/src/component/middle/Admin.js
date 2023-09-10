import React, { useEffect, useState } from 'react';
import axios from './../../axios';
import User from './admin/User';
import EditUser from './admin/EditUser'; // Import the EditUser component
import { useSelector } from 'react-redux';

export default function Admin() {
  const [users, setUsers] = useState([]);
  const [editingUser, setEditingUser] = useState(null); // Track the user being edited
  const tokens = useSelector((state) => state.token);

  useEffect(() => {
    displayData();
  }, []);

  function displayData() {
    axios.get("/User/GetAll", {
      headers: {
        Authorization: `Bearer ${tokens.accessToken}`
      }
    })
    .then(response => {
      setUsers(response.data.data);
    })
    .catch(error => {
      console.log(error.message);
    });
  }

  function deleteUser(userId) {
    axios.delete(`/User/${userId}`, {
      headers: {
        Authorization: `Bearer ${tokens.accessToken}`
      }
    })
    .then(response => {
      console.log("User deleted successfully");
      displayData(); // Refresh the user list
    })
    .catch(error => {
      console.log(error.message);
    });
  }

  function editUser(user) {
    setEditingUser(user); // Set the user being edited
  }

  function saveUserChanges(updatedUser) {
    // Make the API call to update the user's information
    axios.put(`/User/${updatedUser.userId}`, updatedUser, {
      headers: {
        Authorization: `Bearer ${tokens.accessToken}`
      }
    })
    .then(response => {
      console.log("User updated successfully");
      setEditingUser(null); // Reset the editing state
      displayData(); // Refresh the user list
    })
    .catch(error => {
      console.log(error.message);
    });
  }


  return (
    <div>
    <h1 className="text-center">Admin Panel</h1>
    <h4>List of Users</h4>
    {editingUser ? (
      <EditUser user={editingUser} onSave={saveUserChanges} />
    ) : (
      <div className="card m-3 border-primary">
        <div className="card-body">
          <table className="table table-hover table-striped table-bordered">
            <thead>
              <tr>
                <th scope="col">UserId</th>
                <th scope="col">Role</th>
                <th scope="col">First Name</th>
                <th scope="col">Email</th>
                <th scope="col">Mobile Number</th>
                <th scope="col">Password</th>
                <th scope="col">Is Active</th>
                <th scope="col">Profile Pic</th>
                <th scope="col">Birth Date</th>
                <th scope="col">Email Verify</th>
                <th scope="col">Mobile Verify</th>
                <th scope="col">Edit</th>
                <th scope="col">Delete</th>
              </tr>
            </thead>
            <tbody>
              {users.map(user => (
                <User key={user.userId} user={user}  onDelete={deleteUser} onEdit={editUser} /> // Render the User component for each user
              ))}
            </tbody>
          </table>
          </div>
        </div>
      )}
    </div>
  );
}
