import React from 'react';

export default function User({ user, onEdit, onDelete  }) {
  
  return (
    <>
        <tr key={user.userId} class="table-primary">
          <td>{user.userId}</td>
          <td>{user.role}</td>
          <td>{user.firstName}</td>
          <td>{user.email}</td>
          <td>{user.mobileNumber}</td>
          <td>{user.password}</td>
          <td>{user.isActive}</td>
          <td>{user.profilePic}</td>
          <td>{user.birthDate}</td>
          <td>{user.isEmailVerify}</td>
          <td>{user.isMobileNumberVerify}</td>
          <td>{user.notificaton}</td>
          <td>
          <button type="button" className="btn btn-info" onClick={() => onEdit(user)}>
            Edit
          </button>
        </td>
          <td>
        <button
          type="button"
          className="btn btn-danger"
          onClick={() => onDelete(user.userId)}
        >
          Delete
        </button>
      </td>
        </tr>
    </>
  );
}
