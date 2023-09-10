import React, { useEffect, useState } from 'react';

export default function EditUser({ user, onSave }) {
  const [editedUser, setEditedUser] = useState(user); 
  const [role, setRole] = useState(user.role);
  const [firstName, setFirstName] = useState(user.firstName);
  const [lastName, setLastName] = useState(user.firstName);
  const [email, setEmail] = useState(user.email);
  const [mobileNumber, setMobileNumber] = useState(user.mobileNumber);
  const [password, setPassword] = useState(user.password);
  const [isActive, setIsActive] = useState(user.isActive);
  const [profilePic, setProfilePic] = useState('');
  const [birthDate, setBirthDate] = useState(user.birthDate);
  const [isEmailVerify, setIsEmailVerify] = useState(user.isEmailVerify);
  const [isMobileNumberVerify, setIsMobileNumberVerify] = useState(
    user.isMobileNumberVerify
  );
  const [notification, setNotification] = useState(user.notification);

  const roleOptions = ['student', 'teacher', 'admin'];
  const isActiveOptions = ['yes', 'no'];
  const emailVerifyOptions = ['yes', 'no'];
  const notificationOptions = ['none', 'email', 'whatsapp', 'both'];

  useEffect(() => {
    setEditedUser({
      ...editedUser,
      role,
      firstName,
      lastName,
      email,
      mobileNumber,
      password,
      isActive,
      profilePic,
      birthDate,
      isEmailVerify,
      isMobileNumberVerify,
      notification,
    });
  }, [
    role,
    firstName,
    lastName,
    email,
    mobileNumber,
    isActive,
    password,
    profilePic,
    birthDate,
    isEmailVerify,
    isMobileNumberVerify,
    notification,
  ]);

  function handleSave() {
    onSave(editedUser); // Call the onSave function with the edited user
  }

  return (
    <><div className="card m-3 border-primary">
    <div className="card-body">
      <h1>Edit User</h1>
      <h4>
        Role :{' '}
        <select
          value={role}
          onChange={(e) => setRole(e.target.value)}
          className="form-control"
        >
          {roleOptions.map((option) => (
            <option key={option} value={option}>
              {option}
            </option>
          ))}
        </select>
        First Name :{' '}
        <input
          type="text"
          className="form-control"
          value={firstName}
          onChange={(e) => setFirstName(e.target.value)}
        />
        <i class="fa fa-lastfm" aria-hidden="true"></i> Name :{' '}
        <input
          type="text"
          className="form-control"
          value={lastName}
          onChange={(e) => setLastName(e.target.value)}
        />
        Email :{' '}
        <input
          type="text"
          className="form-control"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
        Mobile Number :{' '}
        <input
          type="text"
          className="form-control"
          value={mobileNumber}
          onChange={(e) => setMobileNumber(e.target.value)}
        />
        Password :{' '}
        <input
          type="password"
          className="form-control"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        Is Active :{' '}
        <select
          value={isActive}
          onChange={(e) => setIsActive(e.target.value)}
          className="form-control"
        >
          {isActiveOptions.map((option) => (
            <option key={option} value={option}>
              {option}
            </option>
          ))}
        </select>
        Profile Pic :{' '}
        <input
          type="file"
          className="form-control-file"
          onChange={(e) => setProfilePic(e.target.files[0])}
        />
        Birth Date :{' '}
        <input
          type="date"
          className="form-control"
          value={birthDate}
          onChange={(e) => setBirthDate(e.target.value)}
        />
        Email Verify :{' '}
        <select
          value={isEmailVerify}
          onChange={(e) => setIsEmailVerify(e.target.value)}
          className="form-control"
        >
          {emailVerifyOptions.map((option) => (
            <option key={option} value={option}>
              {option}
            </option>
          ))}
        </select>
        Mobile Number Verify :{' '}
        <select
          value={isMobileNumberVerify}
          onChange={(e) => setIsMobileNumberVerify(e.target.value)}
          className="form-control"
        >
          {emailVerifyOptions.map((option) => (
            <option key={option} value={option}>
              {option}
            </option>
          ))}
        </select>
      </h4>
      <div>
          <h4>Notification Preference:</h4>
          <select
            value={notification}
            onChange={(e) => setNotification(e.target.value)}
            className="form-control"
          >
            {notificationOptions.map((option) => (
              <option key={option} value={option}>
                {option}
              </option>
            ))}
          </select>
        </div>
      <button type="button" className="btn btn-success" onClick={handleSave}>
        Save
      </button>
      </div></div>
    </>
  );
}
