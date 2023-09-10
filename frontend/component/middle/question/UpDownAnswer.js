import { useDispatch } from 'react-redux';
import { setAnswerMode } from './../../../action/settingsSlice';
export default function SideBySide({systemAnswer, userAnswer}){
    const dispatch = useDispatch();
    return(
        <>
            <div className="position-relative mt-3">
                <div className="border p-3 bg-primary">
                    <div className="position-absolute top-custom end-custom1" onClick={() => dispatch(setAnswerMode("SystemAnswer"))}>
                        <svg xmlns="http://www.w3.org/2000/svg" width="27" height="27" fill="currentColor" className="bi bi-file" viewBox="0 0 16 16">
                            <path d="M4 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2H4zm0 1h8a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1z"/>
                        </svg>
                    </div>
                    <h3 className='text-warning'>System Answer</h3>
                    {systemAnswer}
                </div>
                <div className="border p-3 mt-1 bg-primary">
                <h3 className='text-warning'>Your answer</h3>
                {userAnswer}
                </div>
            </div>
        </>
    )
}