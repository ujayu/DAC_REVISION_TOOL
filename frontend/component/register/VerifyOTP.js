import { useState } from 'react';

export default function VerifyOTP({registerData, validate}) {
  let [otp, setOtp] = useState({n1:"",n2:"",n3:"",n4:""});
  function sendData(){
    console.log(otp);
    let dataToSend = { email: registerData.email, otp: otp.n1+otp.n2+otp.n3+otp.n4};
    console.log(dataToSend);
    validate(dataToSend);
  }
  function saveOtp(data){
    setOtp({...otp, ...data})
  }
  return (
    <div className="container height-100 d-flex justify-content-center align-items-center">
      <div className="position-relative">
        <div className="card p-2 text-center">
          <h6>Please enter the one-time password<br />to verify your account</h6>
          <div>
            <span>A code has been sent to</span>
            <small>{registerData.email}</small>
          </div>
          <div id="otp" className="inputs d-flex flex-row justify-content-center mt-2">
            <input className="m-2 text-center form-control rounded" type="text" maxLength="1" onChange={e=>saveOtp({n1 : e.target.value})}/>
            <input className="m-2 text-center form-control rounded" type="text" maxLength="1" onChange={e=>saveOtp({n2 : e.target.value})}/>
            <input className="m-2 text-center form-control rounded" type="text" maxLength="1" onChange={e=>saveOtp({n3 : e.target.value})}/>
            <input className="m-2 text-center form-control rounded" type="text" maxLength="1" onChange={e=>saveOtp({n4 : e.target.value})}/>
          </div>
          <div className="mt-4">
            <div className="btn btn-danger px-4 validate" onClick={sendData}>Validate</div>
          </div>
        </div>
        <div className="card-2">
          <div className="content d-flex justify-content-center align-items-center">
            <span>Didn't get the code</span>
            <a href="a" className="text-decoration-none ms-3">Resend(1/3)</a>
          </div>
        </div>
      </div>
    </div>
  );
}
