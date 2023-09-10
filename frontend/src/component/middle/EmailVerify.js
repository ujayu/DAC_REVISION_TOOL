import axios from './../../axios';
import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate, useParams } from 'react-router-dom';
import { setUser } from '../../action/userSlice';

export default function EmailVerify() {
    const { email } = useParams();
    const [otp, setOtp] = useState(["", "", "", ""]);
    const tokens = useSelector((state) => state.token);
    const [sendMessage, setSendMessage] = useState("hide");
    const [resendCountdown, setResendCountdown] = useState(0); // Countdown timer in seconds
    const [resendDisabled, setResendDisabled] = useState(false);
    const navigate = useNavigate();
    const [success, setSuccess] = useState(false);
    const [fail, setFail] = useState(false);
    const dispatch = useDispatch();

    const handleKeyDown = (event, index) => {
        if (event.key === 'Backspace' && index > 0) {
            const prevInput = event.target.previousSibling;
            if (prevInput) {
                prevInput.focus();
                prevInput.setSelectionRange(0, 1); // Select the previous input's value
            }
        } else if (event.target.value.length === 1 && index < 3) {
            const nextInput = event.target.nextSibling;
            if (nextInput) {
                nextInput.focus();
                nextInput.setSelectionRange(0, 1); // Select the next input's value
            }
        }
    };

    useEffect(() => {
        if (resendCountdown > 0 && resendDisabled) {
            const timer = setTimeout(() => {
                setResendCountdown(prevCountdown => prevCountdown - 1);
            }, 1000);

            return () => clearTimeout(timer);
        } else {
            setResendDisabled(false);
        }
    }, [resendCountdown, resendDisabled]);

    const handleInputChange = (event, index) => {
        const newValue = event.target.value;
        setOtp((prevOtp) => {
            const newOtp = [...prevOtp];
            newOtp[index] = newValue;
            return newOtp;
        });
    };

    const handleFocus = (event) => {
        event.target.select();
    };

    function verifyEmail() {
        const enteredOtp = otp.join('');
        console.log(enteredOtp);
        // Add your verification logic here
        axios.put("/User/VerifyOTPEmail/" + otp[0] + otp[1] + otp[2] + otp[3], null,  {
            headers: {
                Authorization: `Bearer ${tokens.accessToken}`
            }
        })
            .then(response => {
                console.log(response);
                setSuccess(true);
                setFail(false);
                axios.get("/User",{
                    headers: {
                        Authorization: `Bearer ${tokens.accessToken}` 
                    }
                })
                .then(response=>{
                      setTimeout(()=>{
                          navigate("/home");
                      },500)
                    console.log(response.data.data);
                    dispatch(setUser(response.data.data));
                  });
            })
            .catch(error => {
                console.log(error);
                setFail(true);
            });
    }

    function sendOTPEmail() {
        axios.put("/User/SendOTPEmail/"+email, {email : email},{
            headers: {
                Authorization: `Bearer ${tokens.accessToken}`
            }
        })
            .then(response => {
                console.log(response);
                setSendMessage("show");
                setResendCountdown(180);
                setResendDisabled(true);
            })
            .catch(error => {
                console.log(error.response.data);
            });
    }

    return (
        <>
            <div className="container height-100 d-flex justify-content-center align-items-center">
                <div className="position-relative">
                    <div className="card p-2 text-center">
                        <h6>Please enter the one time password <br /> to verify your account</h6>
                        <div> <span>A code has been sent to</span> <small>{email}</small> </div>
                        <div id="otp" className="inputs d-flex flex-row justify-content-center mt-2">
                            {otp.map((value, index) => (
                                <input
                                    key={index}
                                    className="m-2 text-center form-control rounded"
                                    type="text"
                                    maxLength="1"
                                    value={value}
                                    onKeyDown={(e) => handleKeyDown(e, index)}
                                    onChange={(e) => handleInputChange(e, index)}
                                    onFocus={handleFocus}
                                />
                            ))}
                        </div>
                        <div className="mt-4">
                            <button className="btn btn-danger px-4 validate" onClick={verifyEmail}>
                                Validate
                            </button>
                        </div>
                    </div>
                    <div className="content d-flex justify-content-center align-items-center">
                        <span>Didn't get the code</span>
                        {resendDisabled ? (
                            <span className="text-decoration-none ms-3">
                                Resend ({Math.floor(resendCountdown / 60)}:{String(resendCountdown % 60).padStart(2, '0')})
                            </span>
                        ) : (
                            <div className="text-decoration-none ms-3" onClick={sendOTPEmail}>
                                Resend
                            </div>
                        )}
                    </div>
                </div>
            </div>
            <div className='d-flex justify-content-center align-items-center mt-3'>
                <div class={`toast ${sendMessage}`} role="alert" aria-live="assertive" aria-atomic="true">
                        <button type="button" class="btn-close ms-2 mb-1" data-bs-dismiss="toast" aria-label="Close">
                            <span aria-hidden="true"></span>
                        </button>
                    <div class="toast-body">
                        OTP is send to {email}. OTP is valid for only 10 min.
                    </div>
                </div>
            </div>
            {success && <h1 className='text-success text-center'>Validation Completed</h1>}
            {fail && <h1 className='text-danger text-center'>Wrong otp</h1>}
        </>
    );
}
