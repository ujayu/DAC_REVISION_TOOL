import Name from "./register/Name";
import EmailPassword from "./register/EmailPassword";
import React, { useState } from "react";
import axios from "./../axios";
import { useNavigate } from "react-router-dom";
import RegisterSucess from "./register/RegisterSuccess";

export default function Register() {
  let [window, setWindow] = useState("Name");
  let [registerData, setRegisterData] = useState({ firstName: '', lastName: '', email: '', password: '' });
  let [responseMessage, setResponseMessage] = useState("");
  let [showSuccessMessage, setshowSuccessMessage] = useState(false);
  let [showError, setShowError] = useState(false);
  let navigate = useNavigate();

  async function register() {
    await axios.post('/User/Register', registerData)
      .then((response) => {
        console.log(response);
        setshowSuccessMessage(true);
        setTimeout(function() {
          console.log("Waited for 1 second");
          navigate("/login")
      }, 3000)
      })
      .catch((error) => {
        console.log(error);
        setShowError(true);
        setResponseMessage(error.response.data.message);
      });
  }

  function saveRegisterData(data){
    console.log(data);
    setRegisterData({...registerData,...data})
    console.log(registerData);
  }

  return (
    <>
      <div className="d-flex justify-content-center">
        <form>
          <h1 className="text-center">Register</h1>
            {!showSuccessMessage && <>
              {window === "Name" && (
                <Name
                  checkName={setWindow}
                  saveRegisterData={saveRegisterData}
                  registerData={registerData}
                />
              )}
              {window === "EmailPassword" && (
                <EmailPassword
                  checkName={setWindow}
                  register={register}
                  saveRegisterData={saveRegisterData}
                  registerData={registerData}
                />
              )}
              {showError && <p className="text-danger">{responseMessage}</p>}
            </>}
            {showSuccessMessage && <RegisterSucess />}
        </form>
      </div>
    </>
  );
}
