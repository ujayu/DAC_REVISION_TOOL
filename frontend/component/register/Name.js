import React, { useState } from "react";
import { Link } from "react-router-dom";

export default function Name({ checkName, saveRegisterData}) {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [firstNameValid, setFirstNameValid] = useState(false);
  const [lastNameValid, setLastNameValid] = useState(false);

  function validateName(name, setValidFunc) {
    if (/^[A-Za-z]{3,}$/.test(name)) {
      setValidFunc(true);
    } else {
      setValidFunc(false);
    }
  }

  function next() {
    if (firstNameValid && lastNameValid) {
      saveRegisterData({ firstName, lastName });
      checkName("EmailPassword");
    }
  }

  function sendData(data) {
    saveRegisterData(data);
  }

  return (
    <>
      <div className="form-group">
        <div className="form-floating mb-4">
          <input
            type="text"
            className={`form-control ${firstNameValid ? "" : "is-invalid"}`}
            id="floatingFirstName"
            placeholder="John"
            onChange={(e) => {
              sendData({firstName:e.target.value});
              setFirstName(e.target.value);
              validateName(e.target.value, setFirstNameValid);
            }}
            onBlur={() => validateName(firstName, setFirstNameValid)}
          />
          <label htmlFor="floatingFirstName">First Name</label>
          {firstNameValid || firstName === "" ? (
            ""
          ) : (
            <div className="invalid-feedback">First name is invalid.</div>
          )}
        </div>
        <div className="form-floating mb-4">
          <input
            type="text"
            className={`form-control ${lastNameValid ? "" : "is-invalid"}`}
            id="floatingLastName"
            placeholder="Doe"
            onChange={(e) => {
              sendData({lastName:e.target.value});
              setLastName(e.target.value);
              validateName(e.target.value, setLastNameValid);
            }}
            onBlur={() => validateName(lastName, setLastNameValid)}
          />
          <label htmlFor="floatingLastName">Last Name</label>
          {lastNameValid || lastName === "" ? (
            ""
          ) : (
            <div className="invalid-feedback">Last name is invalid.</div>
          )}
        </div>
      </div>

      <div className="text-center">
        <button
          type="button"
          className="btn btn-primary"
          onClick={next}
          disabled={!firstNameValid || !lastNameValid}
        >
          Next
        </button>
      </div>

      <div className="text-center">
        <p>
          Already a member? <Link to="/login">Login</Link>
        </p>
      </div>
    </>
  );
}
