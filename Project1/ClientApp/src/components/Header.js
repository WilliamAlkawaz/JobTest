import React from 'react'
import Button from './Button'

const Header = ({title,onAdd1,onEdit,showAdd,showEdit}) => {
    return (
        <header className='header'>
            <h1>{title}</h1>
            {showAdd?'':<Button text={showEdit?'Cancel':'Edit Categories Ranges'} color={showEdit?'red':'green'} onFun={onEdit} />}
            {showEdit?'':<Button text={showAdd?'Cancel':'Add Category'} color={showAdd?'red':'green'} onFun={onAdd1} />} 
        </header>
    )
}

export default Header
