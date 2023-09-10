import React, { useState, useEffect } from "react";

export default function EmailPassword({ checkName, register, saveRegisterData }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [emailTouched, setEmailTouched] = useState(false);
  const [passwordTouched, setPasswordTouched] = useState(false);
  const [confirmPasswordTouched, setConfirmPasswordTouched] = useState(false);
  const [emailValid, setEmailValid] = useState(false);
  const [passwordValid, setPasswordValid] = useState(false);
  const [confirmPasswordValid, setConfirmPasswordValid] = useState(false);

  useEffect(() => {
    if (emailTouched) {
      validateEmail(email);
    }
    if (passwordTouched) {
      validatePassword(password);
    }
    if (confirmPasswordTouched) {
      validateConfirmPassword(confirmPassword);
    }
  }, [email, password, confirmPassword, emailTouched, passwordTouched, confirmPasswordTouched]);

  function validateEmail(email) {
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    setEmailValid(emailPattern.test(email));
  }

  function validatePassword(password) {
    setPasswordValid(/^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/.test(password));
  }

  function validateConfirmPassword(confirmPassword) {
    setConfirmPasswordValid(confirmPassword === password);
  }

  function previous() {
    checkName("Name");
  }

  function sendData(data) {
    saveRegisterData(data);
  }

  const isRegisterDisabled = !emailValid || !passwordValid || !confirmPasswordValid;

  return (
    <>
      <div className="form-group">
        <div className="form-floating mb-3">
          <input
            type="email"
            className={`form-control ${emailTouched && !emailValid ? "is-invalid" : ""}`}
            id="floatingInput"
            placeholder="name@example.com"
            onChange={(e) => {
              sendData({email:e.target.value});
              setEmail(e.target.value);
              if (!e.target.value) {
                setEmailValid(false);
              } else {
                validateEmail(e.target.value);
              }
            }}
            onBlur={() => setEmailTouched(true)}
          />
          <label htmlFor="floatingInput">Email address</label>
          {emailTouched && !emailValid && (
            <div className="invalid-feedback">Please enter a valid email address.</div>
          )}
        </div>
        <div className="form-floating mb-4">
          <input
            type="password"
            className={`form-control ${passwordTouched && !passwordValid ? "is-invalid" : ""}`}
            id="floatingPassword"
            placeholder="Password"
            autoComplete="off"
            onChange={(e) => {
              setPassword(e.target.value);
              if (!e.target.value) {
                setPasswordValid(false);
              } else {
                validatePassword(e.target.value);
              }
            }}
            onBlur={() => setPasswordTouched(true)}
          />
          <label htmlFor="floatingPassword">Password</label>
          {passwordTouched && !passwordValid && (
            <div className="invalid-feedback">
              Password must be at least 8 characters long and contain at least:
              <br />
              - One letter
              <br />
              - One number
              <br />
              - One special character
            </div>
          )}
        </div>
        <div className="form-floating mb-4">
          <input
            type="password"
            className={`form-control ${
              confirmPasswordTouched && !confirmPasswordValid ? "is-invalid" : ""
            }`}
            id="floatingConformPassword"
            placeholder="Password"
            autoComplete="off"
            onChange={(e) => {
              sendData({password:e.target.value});
              setConfirmPassword(e.target.value);
              if (!e.target.value) {
                setConfirmPasswordValid(false);
              } else {
                validateConfirmPassword(e.target.value);
              }
            }}
            onBlur={() => setConfirmPasswordTouched(true)}
          />
          <label htmlFor="floatingConformPassword">Confirm Password</label>
          {confirmPasswordTouched &&
            confirmPassword !== password &&
            confirmPassword !== "" && (
              <div className="invalid-feedback">Passwords do not match.</div>
          )}
        </div>
      </div>

      <div className="text-center">
        <button type="button" className="btn btn-primary mb-4" onClick={previous}>
          Previous
        </button>
      </div>

      <div className="text-center d-grid">
        <button
          type="button"
          className="btn btn-primary btn-block mb-4"
          onClick={register}
          disabled={isRegisterDisabled}
        >
          Register
        </button>
      </div>
    </>
  );
}
