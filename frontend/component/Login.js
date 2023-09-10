import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import axios from "./../axios";
import { setTokens } from "../action/tokenSlice";
import LoginSucess from "./login/LoginSucess"
import { setUser } from "../action/userSlice";

export default function Login() {
  const navigate = useNavigate();
  const dispatch = useDispatch();


  const [showSuccessMessage, setshowSuccessMessage] = useState(false);
  const [showFailureMessage, setShowFailureMessage] = useState("");

  function login() {
    setShowFailureMessage("");
    axios
      .post("/User/Login", loginCredential)
      .then((response) => {
        const token = {
          accessToken: response.data.data.token,
          refreshToken: response.data.data.refreshToken,
        };
        setshowSuccessMessage(true);
        dispatch(setTokens(token));

        axios.get("/User",{
          headers: {
            Authorization: `Bearer ${response.data.data.token}` // Use the token from the response
          }
        })
        .then(response=>{
          console.log(response.data.data);
          dispatch(setUser(response.data.data));
        })

        setTimeout(() => {
          navigate("/home");
        }, 1000);
      })
      .catch((error) => {
        setShowFailureMessage(error.message);
      });
  }

  const [loginCredential, setLoginCredential] = useState({
    email: "",
    password: "",
  });

  function saveLoginCredential(data) {
    setLoginCredential({ ...loginCredential, ...data });
  }

  return (
    <>
      <div className="d-flex justify-content-evenly">
        <form>
          <h1 className="text-center">Login</h1>

          {!showSuccessMessage && (
            <>
              <div className="form-group">
                <div className="form-floating mb-3">
                  <input
                    type="email"
                    className="form-control"
                    id="floatingInput"
                    placeholder="name@example.com"
                    onChange={(e) =>
                      saveLoginCredential({ email: e.target.value })
                    }
                  />
                  <label htmlFor="floatingInput">Email address</label>
                </div>
                <div className="form-floating mb-4">
                  <input
                    type="password"
                    className="form-control"
                    id="floatingPassword"
                    placeholder="Password"
                    autoComplete="off"
                    onChange={(e) =>
                      saveLoginCredential({ password: e.target.value })
                    }
                  />
                  <label htmlFor="floatingPassword">Password</label>
                </div>
              </div>

              <div className="row mb-4">
                <div className="col">
                  <a href="#!">Forgot password?</a>
                </div>
              </div>

              <div className="text-center d-grid">
                <div
                  type="button"
                  className="btn btn-primary btn-block mb-4"
                  onClick={login}
                >
                  Login
                </div>
              </div>

              <div className="text-center">
                <p>
                  Not a member? <Link to="/register">Register</Link>
                </p>
              </div>
            </>
          )}
          {showSuccessMessage && <LoginSucess />}
          <div className="text-danger">{showFailureMessage}</div>
        </form>
      </div>
    </>
  );
}