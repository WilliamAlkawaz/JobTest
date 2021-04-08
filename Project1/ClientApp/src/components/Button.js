import React from 'react'

const Button = ({color,text,onFun}) => {
    return (
        <button style={{backgroundColor: color}} className='btn' onClick={onFun}>{text}</button>
    )
}

export default Button
